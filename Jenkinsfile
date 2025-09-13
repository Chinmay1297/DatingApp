pipeline {
  agent any

  environment {
    DOTNET_ROOT = '/root/.dotnet'
    PATH = "/root/.dotnet:/root/.dotnet/tools:${env.PATH}"
    DOCKERHUB = credentials('dockerhub-creds') // DockerHub creds
    IMAGE = 'chinmay1297/datequest' // Docker image name
    AZURE_STORAGE_ACCOUNT = 'datequeststorage' // Your storage account name
    AZURE_STORAGE_KEY = credentials('AZURE_STORAGE_KEY') // Add this in Jenkins UI as secret text
    BUILD_DIR = 'client/dist/client' // Angular build output
    SONAR_TOKEN = credentials('SONAR_TOKEN') // SonarQube token injected securely
  }

  tools {
    nodejs 'Node_16' // Ensure this is configured in Jenkins UI
  }

  stages {
    stage('Checkout') {
      steps {
        git branch: 'datequest-ci-cd', url: 'https://github.com/Chinmay1297/DatingApp.git'
      }
    }

    stage('Restore .NET') {
      steps {
        dir('API') {
          sh 'dotnet restore'
        }
      }
    }

    stage('SonarQube Analysis') {
      steps {
        withSonarQubeEnv('LocalSonarQube') {
          withCredentials([string(credentialsId: 'SONAR_TOKEN', variable: 'SONAR_AUTH')]) {
            dir('API') {
              sh 'dotnet sonarscanner begin /k:"datequest" /d:sonar.host.url="http://sonarqube:9000" /d:sonar.login="$SONAR_AUTH"'
              sh 'dotnet build --configuration Release'
              sh 'dotnet sonarscanner end /d:sonar.login="$SONAR_AUTH"'

            }
          }
        }
      }
    }

    // stage('Build .NET') {
    //   steps {
    //     dir('API') {
    //       sh 'dotnet build --configuration Release'
    //     }
    //   }
    // }

    stage('Publish .NET') {
      steps {
        dir('API') {
          sh 'dotnet publish --configuration Release --output out'
        }
      }
    }

    // Uploading to Azure Blob Storage (commented out as subscription is expired)
    // stage('Build Angular') {
    //   steps {
    //     dir('client') {
    //       sh 'npm install --legacy-peer-deps'
    //       sh 'npm run build -- --configuration=production'
    //     }
    //   }
    // }

    // stage('Upload to Azure Blob Storage') {
    //   steps {
    //     withCredentials([string(credentialsId: 'AZURE_STORAGE_KEY', variable: 'AZURE_KEY')]) {
    //       sh '''
    //         CONTAINER_NAME="\\$web"
    //         az storage blob upload-batch \
    //           --account-name datequeststorage \
    //           --account-key "$AZURE_KEY" \
    //           --destination "$CONTAINER_NAME" \
    //           --source client/dist/client \
    //           --overwrite true
    //       '''
    //     }
    //   }
    // }

    // Optional Docker stage (commented out)
    // stage('Docker Build & Push') {
    //   steps {
    //     script {
    //       dir('API') {
    //         docker.withRegistry('', 'dockerhub-creds') {
    //           def img = docker.build("${IMAGE}:${env.BUILD_NUMBER}", '.')
    //           img.push()
    //           img.push('latest')
    //         }
    //       }
    //     }
    //   }
    // }
  }

  post {
    success {
      echo '✅ Build and deployment completed successfully.'
    }
    failure {
      echo '❌ Something went wrong. Check the logs.'
    }
  }
}