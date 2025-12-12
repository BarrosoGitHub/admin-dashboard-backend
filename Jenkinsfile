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
                [key: 'REFID', value: 'push.changes[0].new.name'], 
                [key: 'REPONAME', value: 'repository.name'],
                [key: 'PROJKEY', value: 'repository.project.key']
                [key: 'COMMITHASH', value: 'push.changes[0].new.target.hash'],

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
    ERROR_MSG="\nFormato de TAGNAME inválido: ${TAGNAME}. \nFormatos validos de TAGs. \nEx: 1.0.0.0, 1.0.0.0-x86, 1.0.0.0-fuel, 1.0.0.0-dev, 1.0.0.0-dev-x86, 1.0.0.0-qa, 1.0.0.0-qa-x86 \n"
    DOCKER_BASE='docker buildx build --sbom=false --provenance=false --push '
    DOCKER_FILE=' -f Dockerfile '
    DOCKER_FILE_X86=' -f Dockerfile.x86 '
    DOCKER_NEXUS_PROD="  -t ${env.NEXUS_URL}${env.NEXUS_PORT_PROD}/${REPONAME}:${TAGNAME} "
    DOCKER_NEXUS_DEV="  -t ${env.NEXUS_URL}${env.NEXUS_PORT_DEV}/${REPONAME}:${TAGNAME} "
    DOCKER_NEXUS_QA="  -t ${env.NEXUS_URL}${env.NEXUS_PORT_QA}/${REPONAME}:${TAGNAME} "
    DOCKER_AWS=" -t ${env.AWS_ECR_URL}/${REPONAME}:${TAGNAME} "
  }	  
  stages {
    stage('Push Docker Images to Nexus Registry and AWS ECR') {
      steps {
        script {
          // Executa o comando git e captura o resultado
          def gitOutput = sh(script: "git branch --contains '${REFID}'", returnStdout: true).trim()
          // Usar regex para capturar o commit hash
          def commitHash = env.COMMITHASH

          def branchName = sh(script: "git branch -r --contains ${commitHash} | grep -oE '[^/]+\$'", returnStdout: true).trim() 
          
          env.DOCKER_VERSAO=" --build-arg 'versao=${branchName}-${TAGNAME}'"
          
          // Detectar se é x86 e definir o Dockerfile apropriado
          def dockerFile = env.DOCKER_FILE
          if (TAGNAME.endsWith('-x86')) {
            dockerFile = env.DOCKER_FILE_X86
          }

          withCredentials([usernamePassword(credentialsId: 'nexus_3_docker', passwordVariable: 'pass', usernameVariable: 'user')]) { 
              // Validação e determinação do ambiente com base no TAGNAME          
              if (TAGNAME ==~ /^\d+\.\d+\.\d+\.\d+-dev(-x86)?$/) {
                sh """
                  docker login -u $user -p $pass ${env.NEXUS_PROTOCOL}${env.NEXUS_URL}${env.NEXUS_PORT_DEV}/repository/docker-private/
                  ${env.DOCKER_BASE} ${env.DOCKER_VERSAO} ${dockerFile} ${env.DOCKER_NEXUS_DEV} .
                """						
              } else if (TAGNAME ==~ /^\d+\.\d+\.\d+\.\d+-qa(-x86)?$/) {						
                sh """
                  docker login -u $user -p $pass ${env.NEXUS_PROTOCOL}${env.NEXUS_URL}${env.NEXUS_PORT_QA}/repository/docker-private/
                  ${env.DOCKER_BASE} ${env.DOCKER_VERSAO} ${dockerFile} ${env.DOCKER_NEXUS_QA}  ${env.DOCKER_AWS} .
                """						
              } else if (TAGNAME ==~ /^\d+\.\d+\.\d+\.\d+(-x86)?$/) {						 
                sh """
                  docker login -u $user -p $pass ${env.NEXUS_PROTOCOL}${env.NEXUS_URL}${env.NEXUS_PORT_PROD}/repository/docker-private/
                  ${env.DOCKER_BASE} ${env.DOCKER_VERSAO} ${dockerFile} ${env.DOCKER_NEXUS_PROD} ${env.DOCKER_AWS} .
                """
              } else if (TAGNAME ==~ /^\d+\.\d+\.\d+\.\d+-[a-zA-Z]+(-x86)?$/) {            
                sh """ 
                  docker login -u $user -p $pass ${env.NEXUS_PROTOCOL}${env.NEXUS_URL}${env.NEXUS_PORT_PROD}/repository/docker-private/ 
                  ${env.DOCKER_BASE} ${env.DOCKER_VERSAO} ${dockerFile} ${env.DOCKER_NEXUS_PROD} ${env.DOCKER_AWS} .
                """						
              } 
              else {
               error "${env.ERROR_MSG}"						
              }
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