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
       token: 'opt-configurator',
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
    NUGET_URL = 'http://172.16.50.65:8081/repository/nuget/index.json'
    ERROR_MSG = "\nFormato de TAGNAME inválido: ${TAGNAME}. \nFormatos validos de TAGs. \nEx: 1.0.0.0, 1.0.0.0-fuel, 1.0.0.0-dev, 1.0.0.0-qa \n"
    DOCKER_BASE = 'docker buildx build --sbom=false --provenance=false --push '
    DOCKER_FILE = ' -f Dockerfile '
    DOCKER_NEXUS_PROD = "  -t ${env.NEXUS_URL}${env.NEXUS_PORT_PROD}/${REPONAME}:${TAGNAME} "
    DOCKER_NEXUS_DEV = "  -t ${env.NEXUS_URL}${env.NEXUS_PORT_DEV}/${REPONAME}:${TAGNAME} "
    DOCKER_NEXUS_QA = "  -t ${env.NEXUS_URL}${env.NEXUS_PORT_QA}/${REPONAME}:${TAGNAME} "
    DOCKER_AWS = " -t ${env.AWS_ECR_URL}/${REPONAME}:${TAGNAME} "
    DOTNET_VERSION = 'net9.0'
    PROJECT_NAME = 'OPTConfigurator'
  }
  
  stages {
    stage('Checkout') {
      steps {
        checkout scm
      }
    }
    
    stage('Restore Dependencies') {
      steps {
        script {
          sh """
            dotnet restore ${PROJECT_NAME}.csproj --source ${NUGET_URL} --source https://api.nuget.org/v3/index.json
          """
        }
      }
    }
    
    stage('Build Application') {
      steps {
        script {
          sh """
            dotnet build ${PROJECT_NAME}.csproj --configuration Release --no-restore
          """
        }
      }
    }
    
    stage('Run Tests') {
      steps {
        script {
          // Run tests if test projects exist
          sh """
            if [ -d "Tests" ] || find . -name "*.Tests.csproj" -o -name "*Test*.csproj" | grep -q .; then
              dotnet test --configuration Release --no-build --verbosity normal --logger trx --results-directory TestResults/
            else
              echo "No test projects found, skipping tests"
            fi
          """
        }
      }
      post {
        always {
          script {
            // Publish test results if they exist
            if (fileExists('TestResults/*.trx')) {
              publishTestResults testResultsPattern: 'TestResults/*.trx'
            }
          }
        }
      }
    }
    
    stage('Publish Application') {
      steps {
        script {
          sh """
            dotnet publish ${PROJECT_NAME}.csproj --configuration Release --output ./publish --no-build --verbosity normal
          """
        }
      }
    }
    
    stage('Push Docker Images to Nexus Registry and AWS ECR') {
      steps {
        script {
          // Get branch information
          def gitOutput = sh(script: "git branch --contains '${REFID}'", returnStdout: true).trim()
          def commitHash = (gitOutput =~ /detached at ([a-f0-9]{7,40})/)[0][1]
          def branchName = sh(script: "git branch -r --contains ${commitHash} | grep -oE '[^/]+\$'", returnStdout: true).trim() 
          
          env.DOCKER_VERSAO = " --build-arg 'versao=${branchName}-${TAGNAME}'"         

          withCredentials([usernamePassword(credentialsId: 'nexus_3_docker', passwordVariable: 'pass', usernameVariable: 'user')]) { 
              // AWS ECR login for QA and Production environments
              script {
                if (TAGNAME ==~ /^\d+\.\d+\.\d+\.\d+-qa$/ || TAGNAME ==~ /^\d+\.\d+\.\d+\.\d+$/ || TAGNAME ==~ /^\d+\.\d+\.\d+\.\d+-[a-zA-Z]+$/) {
                  withCredentials([[$class: 'AmazonWebServicesCredentialsBinding', credentialsId: 'aws-ecr-credentials']]) {
                    sh """
                      aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin ${env.AWS_ECR_URL}
                    """
                  }
                }
              }
              
              // Build and push based on tag format
              if (TAGNAME ==~ /^\d+\.\d+\.\d+\.\d+-dev$/) {
                sh """
                  docker login -u $user -p $pass ${env.NEXUS_PROTOCOL}${env.NEXUS_URL}${env.NEXUS_PORT_DEV}/repository/docker-private/
                  ${env.DOCKER_BASE} ${env.DOCKER_VERSAO} ${env.DOCKER_FILE} ${env.DOCKER_NEXUS_DEV} .
                """						
              } else if (TAGNAME ==~ /^\d+\.\d+\.\d+\.\d+-qa$/) {						
                sh """
                  docker login -u $user -p $pass ${env.NEXUS_PROTOCOL}${env.NEXUS_URL}${env.NEXUS_PORT_QA}/repository/docker-private/
                  ${env.DOCKER_BASE} ${env.DOCKER_VERSAO} ${env.DOCKER_FILE} ${env.DOCKER_NEXUS_QA} ${env.DOCKER_AWS} .
                """						
              } else if (TAGNAME ==~ /^\d+\.\d+\.\d+\.\d+$/) {						 
                sh """
                  docker login -u $user -p $pass ${env.NEXUS_PROTOCOL}${env.NEXUS_URL}${env.NEXUS_PORT_PROD}/repository/docker-private/
                  ${env.DOCKER_BASE} ${env.DOCKER_VERSAO} ${env.DOCKER_FILE} ${env.DOCKER_NEXUS_PROD} ${env.DOCKER_AWS} .
                """
              } else if (TAGNAME ==~ /^\d+\.\d+\.\d+\.\d+-[a-zA-Z]+$/) {            
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
      // Archive build artifacts
      archiveArtifacts artifacts: 'publish/**/*', fingerprint: true, allowEmptyArchive: true
      
      // Clean workspace
      cleanWs()
    }
    
    success {
      office365ConnectorSend(
          status: "SUCCESS",
          webhookUrl: "${env.MSTEAMS_HOOK_SAINSBURYS}",
          message: "✅ Build SUCCESS: ${JOB_NAME} - ${BUILD_DISPLAY_NAME}<br>Pipeline duration: ${currentBuild.durationString}<br>Tag: ${TAGNAME}"
      )
    }
    
    failure {
      office365ConnectorSend(
          status: "FAILURE",
          webhookUrl: "${env.MSTEAMS_HOOK_SAINSBURYS}",
          message: "❌ Build FAILED: ${JOB_NAME} - ${BUILD_DISPLAY_NAME}<br>Pipeline duration: ${currentBuild.durationString}<br>Tag: ${TAGNAME}"
      )
    }
    
    unstable {
      office365ConnectorSend(
          status: "UNSTABLE",
          webhookUrl: "${env.MSTEAMS_HOOK_SAINSBURYS}",
          message: "⚠️ Build UNSTABLE: ${JOB_NAME} - ${BUILD_DISPLAY_NAME}<br>Pipeline duration: ${currentBuild.durationString}<br>Tag: ${TAGNAME}"
      )
    }
  }
}