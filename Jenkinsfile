pipeline {
  agent any
  
    triggers {
    GenericTrigger(
       genericVariables: [
            [key: 'ADDTYPE', value: 'push.changes[0].new.target.type'],
                [key: 'TAGTYPE', value: 'push.changes[0].new.type'],
                [key: 'TAGNAME', value: 'push.changes[0].new.name'],
                [key: 'AUTOR', value: 'push.changes[0].new.target.author.raw'],                 
                [key: 'DATE', value: 'push.changes[0].new.target.date'],
                [key: 'PROJECTNAME', value: 'repository.project.name'],
                [key: 'REPO', value: 'repository.full_name'],
                [key: 'REFID', value: 'push.changes[0].new.links.self.href', regexpFilter: '^.*(?=refs\\/tags\\/)'],                 
                [key: 'REPONAME', value: 'repository.name'],
                [key: 'PROJKEY', value: 'repository.project.key']
       ],
       causeString: 'Generic Cause',
       token: 'opt-emotion-configurator-api',
       tokenCredentialId: '',
       printContributedVariables: true,
       printPostContent: true,
       silentResponse: true,
       regexpFilterText: '$TAGTYPE#$ADDTYPE',
       regexpFilterExpression: 'tag#commit'
    )
  }  	
    options {
    buildDiscarder logRotator(artifactDaysToKeepStr: '', artifactNumToKeepStr: '', daysToKeepStr: '', numToKeepStr: '10')
  }
  
  
  environment {
    NEXUS_PROTOCOL = 'http://'
    NEXUS_URL = '172.16.50.65'    
    NEXUS_PORT_PROD = ':8082'
    NEXUS_PORT_DEV = ':8083'
    NEXUS_PORT_QA = ':8084'
    NUGET_URL='http://172.16.50.65:8081/repository/nuget/index.json'
    ERROR_MSG="\nFormato de TAGNAME inválido: ${TAGNAME}. \nFormatos validos de TAGs. \nEx: 1.0.0.0, 1.0.0.0-fuel, 1.0.0.0-dev, 1.0.0.0-qa \n"
    DOCKER_BASE='docker buildx build --sbom=false --provenance=false --push '
    DOCKER_FILE_ARM=' -f Dockerfile '
    DOCKER_FILE_AMD64=' -f Dockerfile.amd64 '
    DOCKER_FILE_AOT=' -f Dockerfile.aot '
  }	  
  stages {
    stage('Push Docker Images to Nexus Registry and AWS ECR') {
      steps {
        script {
          // Executa o comando git e captura o resultado
          def gitOutput = sh(script: "git branch --contains '${REFID}'", returnStdout: true).trim()
          // Usar regex para capturar o commit hash
          def commitHash = (gitOutput =~ /detached at ([a-f0-9]{7,40})/)[0][1]
          def branchName = sh(script: "git branch -r --contains ${commitHash} | grep -oE '[^/]+\$'", returnStdout: true).trim() 
          
          env.DOCKER_VERSAO=" --build-arg 'versao=${branchName}-${TAGNAME}'"

          withCredentials([usernamePassword(credentialsId: 'nexus_3_docker', passwordVariable: 'pass', usernameVariable: 'user')]) { 
              // Validação e determinação do ambiente com base no TAGNAME
              def nexusPort = ''
              def awsTag = ''
              
              if (TAGNAME ==~ /^\d+\.\d+\.\d+\.\d+-dev$/) {
                nexusPort = env.NEXUS_PORT_DEV
              } else if (TAGNAME ==~ /^\d+\.\d+\.\d+\.\d+-qa$/) {
                nexusPort = env.NEXUS_PORT_QA
                awsTag = " -t ${env.AWS_ECR_URL}/${REPONAME}:${TAGNAME}-arm64-aot -t ${env.AWS_ECR_URL}/${REPONAME}:${TAGNAME}-amd64-aot"
              } else if (TAGNAME ==~ /^\d+\.\d+\.\d+\.\d+$/ || TAGNAME ==~ /^\d+\.\d+\.\d+\.\d+-[a-zA-Z]+$/) {
                nexusPort = env.NEXUS_PORT_PROD
                awsTag = " -t ${env.AWS_ECR_URL}/${REPONAME}:${TAGNAME}-arm64-aot -t ${env.AWS_ECR_URL}/${REPONAME}:${TAGNAME}-amd64-aot"
              } else {
                error "${env.ERROR_MSG}"
              }

              // Build ARM64 image with AOT
              sh """
                docker login -u $user -p $pass ${env.NEXUS_PROTOCOL}${env.NEXUS_URL}${nexusPort}/repository/docker-private/
                ${env.DOCKER_BASE} ${env.DOCKER_VERSAO} ${env.DOCKER_FILE_AOT} \
                  -t ${env.NEXUS_URL}${nexusPort}/${REPONAME}:${TAGNAME}-arm64-aot \
                  ${awsTag ? '-t ' + env.AWS_ECR_URL + '/' + REPONAME + ':' + TAGNAME + '-arm64-aot' : ''} .
              """
              
              // Build amd64 image with AOT
              sh """
                ${env.DOCKER_BASE} ${env.DOCKER_VERSAO} ${env.DOCKER_FILE_AOT} \
                  -t ${env.NEXUS_URL}${nexusPort}/${REPONAME}:${TAGNAME}-amd64-aot \
                  ${awsTag ? '-t ' + env.AWS_ECR_URL + '/' + REPONAME + ':' + TAGNAME + '-amd64-aot' : ''} .
              """
          }
        }
      }
    }    
  }
  post {
    always {
      office365ConnectorSend(
          status: "${currentBuild.result}",
          webhookUrl: "${env.MSTEAMS_HOOK_SAINSBURYS}",
          message: "Test ${currentBuild.result}: ${JOB_NAME} - ${BUILD_DISPLAY_NAME}<br>Pipeline duration: ${currentBuild.durationString}"
      )
      cleanWs()
    }
  }
}