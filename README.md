# Serverless.Alert.Proxy
A simple proxy for webhooks alerts from Grafana to rocket.chat application.

## Configuration rocket.chat (_working progress_)
### 1. create a room or user 
First, you need to create a user or a chat room that the webhook integrations forward the messages to. 

### 2. setup the webhook integration
Create new Incoming webhook integrations under _administration_ and _Integrations_ in the rocket. Chat app. 

### 3. clone and deploy
Clone this GitHub repository and deploy it to a new Azure Functions. There are many ways of deploying; one way is to eploy through the VS code. 

### 4. configure Azure Functions
In the _Applications Settings_ in the Azure portal for your newly created Azure Function, add this parameter with your own rocket chat webhook url: rocketchaturl

## Configure Grafana
Add a new channel under the Alert and Notification channels in Grafana. Choose the type "webhook" and enter the Azure function Api URL. 

## MIT License
