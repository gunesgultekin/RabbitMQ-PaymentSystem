# RabbitMQ-PaymentSystem (Using .NET Core)

## System Architecture
![System Architecture](https://github.com/gunesgultekin/RabbitMQ-PaymentSystem/assets/126399958/3dcff51b-cd06-43ca-b3b7-99b264c7eefd)
## Detailed System Architecture
![System Architecture detailed](https://github.com/gunesgultekin/RabbitMQ-PaymentSystem/assets/126399958/68f499a7-71c0-4dbe-a1e1-b08b9e2346ba)

# Step-1 Run APIGateway then call /makePayment
by inputting paymentId and deviceId parameters 
![makepayment-1](https://github.com/gunesgultekin/RabbitMQ-PaymentSystem/assets/126399958/f8e840aa-baab-4de9-bf4e-8389f2d2647b)
![makepayment2](https://github.com/gunesgultekin/RabbitMQ-PaymentSystem/assets/126399958/88feb387-9a59-4a48-bda4-f80810ce38fb)
* Observe that a message has arrived in the "Payments" queue in the RabbitMQ Management Panel (Default: http://localhost:15672/#/queues)
![makepayment3](https://github.com/gunesgultekin/RabbitMQ-PaymentSystem/assets/126399958/eb95659b-483f-498e-95ee-7bdc28fdb662)
  
# Step-2 Run PaymentService then call /processPayment
![processpayment1](https://github.com/gunesgultekin/RabbitMQ-PaymentSystem/assets/126399958/08bc2167-0174-4063-913f-a5b50fb628f9)
* Observe that the message(s) in the "Payments" queue are consumed, and the function that performs payment operations is called according to the parameters from the payments queue.
![processpayment2](https://github.com/gunesgultekin/RabbitMQ-PaymentSystem/assets/126399958/f6d24b2d-e6d5-4635-b040-43ce49e1b28c)
* Payment function sends notification parameters (deviceId, payment status) to the "Notifications" queue for to be processed in Notification Service like Firebase Cloud Messaging.),
according to payment status (successful or failed)

# Step-3 Run NotificationService then call /sendNotification
![notification1](https://github.com/gunesgultekin/RabbitMQ-PaymentSystem/assets/126399958/0247df22-957b-419e-81f6-bf042c271fa1)
* Parameters (deviceId, status) coming from the notifications queue will be processed by the notification function, and positive/negative payment notifications will be sent to the devices in the incoming id.
![notification2](https://github.com/gunesgultekin/RabbitMQ-PaymentSystem/assets/126399958/eb7668fb-6729-4a54-bbed-a110c6e022f2)
* Observe that all the messages within "Notifications" queue are consumed.
