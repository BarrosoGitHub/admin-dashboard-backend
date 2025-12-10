pipeline {
  agent any
  
    triggers {
    GenericTrigger(
       genericVariables: [
            [key: 'ADDTYPE', value: 'changes[0].type'],
            [key: 'TAGTYPE', value: 'changes[0].ref.type'],
            [key: 'TAGNAME', value: 'changes[0].ref.displayId'],
            [key: 'AUTOR', value: 'actor.name'],
            [key: 'AUTOREMAIL', value: 'actor.emailAddress'],
            [key: 'DATE', value: 'date'],
            [key: 'PROJECTNAME', value: 'repository.project.name'],
            [key: 'REPO', value: 'repository.name'],
            [key: 'REFID', value: 'changes[0].ref.id'],
            [key: 'REPONAME', value: 'repository.slug'],
            [key: 'PROJKEY', value: 'repository.project.key']
       ],
       causeString: 'Generic Cause',
       token: 'opt-emotion-configurator-api',
       tokenCredentialId: '',
       printContributedVariables: true,
       printPostContent: true,
       silentResponse: true,
       regexpFilterText: '$TAGTYPE#$ADDTYPE',
       regexpFilterExpression: 'TAG#ADD'
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
          def commitHash = (gitOutput =~ /detached at ([a-f0-9]{7,40})/)[0][1]
          def branchName = sh(script: "git branch -r --contains ${commitHash} | grep -oE '[^/]+\$'", returnStdout: true).trim() 
          
          env.DOCKER_VERSAO=" --build-arg 'versao=${branchName}-${TAGNAME}'"
          
          // Determine which Dockerfile to use based on tag
          def dockerFile = TAGNAME.endsWith('-x86') ? ' -f Dockerfile.x86 ' : ' -f Dockerfile '
          env.DOCKER_FILE = dockerFile

          withCredentials([usernamePassword(credentialsId: 'nexus_3_docker', passwordVariable: 'pass', usernameVariable: 'user')]) { 
              // Validação e determinação do ambiente com base no TAGNAME          
              if (TAGNAME ==~ /^\d+\.\d+\.\d+\.\d+-dev$|^\d+\.\d+\.\d+\.\d+-dev-x86$/) {
                sh """
                  docker login -u $user -p $pass ${env.NEXUS_PROTOCOL}${env.NEXUS_URL}${env.NEXUS_PORT_DEV}/repository/docker-private/
                  ${env.DOCKER_BASE} ${env.DOCKER_VERSAO} ${env.DOCKER_FILE} ${env.DOCKER_NEXUS_DEV} .
                """						
              } else if (TAGNAME ==~ /^\d+\.\d+\.\d+\.\d+-qa$|^\d+\.\d+\.\d+\.\d+-qa-x86$/) {						
                sh """
                  docker login -u $user -p $pass ${env.NEXUS_PROTOCOL}${env.NEXUS_URL}${env.NEXUS_PORT_QA}/repository/docker-private/
                  ${env.DOCKER_BASE} ${env.DOCKER_VERSAO} ${env.DOCKER_FILE} ${env.DOCKER_NEXUS_QA}  ${env.DOCKER_AWS} .
                """						
              } else if (TAGNAME ==~ /^\d+\.\d+\.\d+\.\d+$|^\d+\.\d+\.\d+\.\d+-x86$/) {						 
                sh """
                  docker login -u $user -p $pass ${env.NEXUS_PROTOCOL}${env.NEXUS_URL}${env.NEXUS_PORT_PROD}/repository/docker-private/
                  ${env.DOCKER_BASE} ${env.DOCKER_VERSAO} ${env.DOCKER_FILE} ${env.DOCKER_NEXUS_PROD} ${env.DOCKER_AWS} .
                """
              } else if (TAGNAME ==~ /^\d+\.\d+\.\d+\.\d+-[a-zA-Z]+$|^\d+\.\d+\.\d+\.\d+-[a-zA-Z]+-x86$/) {            
                sh """ 
                  docker login -u $user -p $pass ${env.NEXUS_PROTOCOL}${env.NEXUS_URL}${env.NEXUS_PORT_PROD}/repository/docker-private/ 
                  ${env.DOCKER_BASE} ${env.DOCKER_VERSAO} ${env.DOCKER_FILE} ${env.DOCKER_NEXUS_PROD} ${env.DOCKER_AWS} .
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