pipeline {
  agent any

  environment {
    DOTNET_ROOT = '/root/.dotnet'
    PATH = "/root/.dotnet:${env.PATH}"
    DOCKERHUB = credentials('dockerhub-creds') // Add this in Jenkins UI
    IMAGE = 'chinmay1297/datequest' // Replace with your actual repo name
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
          sh 'npm install'
          sh 'npm run build -- --configuration=production'
        }
      }
    }

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
      echo '✅ Build pipeline completed successfully.'
    }
    failure {
      echo '❌ Something went wrong. Check the logs.'
    }
  }
}