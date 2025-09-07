pipeline {
  agent any

  environment {
    DOTNET_ROOT = '/root/.dotnet'
    PATH = "/root/.dotnet:${env.PATH}"
    DOCKERHUB = credentials('dockerhub-creds') // DockerHub creds
    IMAGE = 'chinmay1297/datequest' // Docker image name
    AZURE_STORAGE_ACCOUNT = 'datequeststorage' // Your storage account name
    AZURE_STORAGE_KEY = credentials('AZURE_STORAGE_KEY') // Add this in Jenkins UI as secret text
    BUILD_DIR = 'client/dist/client' // Angular build output
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

    stage('Build .NET') {
      steps {
        dir('API') {
          sh 'dotnet build --configuration Release'
        }
      }
    }

    stage('Publish .NET') {
      steps {
        dir('API') {
          sh 'dotnet publish --configuration Release --output out'
        }
      }
    }

    stage('Build Angular') {
      steps {
        dir('client') {
          sh 'npm install --legacy-peer-deps'
          sh 'npm run build -- --configuration=production'
        }
      }
    }

    stage('Upload to Azure Blob Storage') {
      steps {
        withCredentials([string(credentialsId: 'AZURE_STORAGE_KEY', variable: 'AZURE_KEY')]) {
          sh '''
            CONTAINER_NAME="$web"
            az storage blob upload-batch \
              --account-name datequeststorage \
              --account-key "$AZURE_KEY" \
              --destination "$CONTAINER_NAME" \
              --source client/dist/client
          '''
        }
      }
    }
    
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