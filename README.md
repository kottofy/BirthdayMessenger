# Birthday Messenger

This project sends a "Happy Birthday" message to teammates with a GIF!

![Sample Message in Teams](/images/birthdaymessagesample.JPG)

## Setup

This project uses the following components:
    - Three Azure Functions on one Azure Function App
    - One Azure Logic App
    - Two Service Bus Queues in one Service Bus Namespace (`people`, `birthdayalert`)
    - One Storage Account with a Container for the Azure Functions storage and one Table (`people`)
    - Giphy API Key

These resources need to be created manually at this time.

### Add Person

The AddPerson Function will take in information regarding a person and store their details in an Azure Storage Table.

It takes an Http POST request with the following attributes:
    - name
    - email
    - month (birthday month as int)
    - day (birthday day as int)
    - code (Azure Function code if functions are deployed to Azure - not necessary when running locally)

### Check Date

The CheckDate Function will check every day (or multiple times a day) at the specified time (in cron expression). It looks for anyone in the `people` Table who has a birthday of today.

This sends a message to the `birthdayalert` queue.

### Add GIF

The AddGIF function receives the message from the `birthdayalert` and calls out to the Giphy API for a birthday gif.

Once complete, a message is sent to the `people` queue with the newly added GIF info.

### Send Message

The Logic App is the final step. It receives the message from the `people` queue and forms a message for Teams.

It includes:
    - When a message is received in a queue Trigger
    - Parse JSON action
    - Post a message as the Flow bot to a channel (Preview) action

![When a message is received in a queue Trigger](/images/queuetrigger.JPG)
![Parse JSON action](/images/parsejsonaction.JPG)
![Post a message as the Flow bot to a channel (Preview) action](/images/postmessageaction.JPG)

The schema for the Parse JSON Action is in the file `/reference/parsejsonschema.json`.

## Disclaimer

No validation was implemented in the making of this project. Use all of this at your own risk <3
