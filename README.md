# Frends.Amqp

TODO: add workflow, local test runned .cmd and radme things

Frends task for sending and receiving AMQP messages.

- [Installing](#installing)
- [Tasks](#tasks)
  - [Create Pdf](#createpdf)
- [License](#license)
- [Building](#building)
- [Contributing](#contributing)
- [Change Log](#change-log)


# Installing
You can install the task via FRENDS UI Task View or you can find the nuget package from the following nuget feed
https://www.myget.org/F/frends-community/api/v3/index.json


Tasks
=====


## AmqpSender

### AmqpConnection


| Property          | Type                      | Description          | Example    
|-------------------|---------------------------|----------------------|----------------------|
| BusUri            | Text                    | The URI for the AMQP message bus, username and key must be url encoded.        |
| QueueOrTopicName  | Text                    | Name of target queue or topic.        |
| linkName          | Text                    | Link name.        |
| timeout           | Int                     | Timeout in seconds for receiving or sending message to the queue.        |
| SearchCertificateBy  | Enum                 | Select whether certificate is used and where it can be found.        |
| issuer            | Text                    | Issuer of certificate.        |
| pfxFilePath       | Text                    | Path where .pfx (certificate) file can be found.        |
| pfxPassword       | Text                    | Password for the certificate.        |



### AmqpMessage


| Property          | Type                      | Description          | Example    
|-------------------|---------------------------|----------------------|----------------------|
| BusUri              | JToken                    | Response        |
| BusUri              | JToken                    | Response        |
| BusUri              | JToken                    | Response        |
| BusUri              | JToken                    | Response        |
| BusUri              | JToken                    | Response        |
| BusUri              | JToken                    | Response        |
| BusUri              | JToken                    | Response        |
| BusUri              | JToken                    | Response        |
| BusUri              | JToken                    | Response        |
| BusUri              | JToken                    | Response        |



### Output


| Property          | Type                      | Description          | Example    
|-------------------|---------------------------|----------------------|----------------------|
| Success              | bool                    | True if message was sent succesfully.        |

## AmqpReceiver

### AmqpConnection


| Property          | Type                      | Description          | Example    
|-------------------|---------------------------|----------------------|----------------------|
| BusUri              | JToken                    | Response        |
| BusUri              | JToken                    | Response        |
| BusUri              | JToken                    | Response        |
| BusUri              | JToken                    | Response        |
| BusUri              | JToken                    | Response        |
| BusUri              | JToken                    | Response        |
| BusUri              | JToken                    | Response        |
| BusUri              | JToken                    | Response        |
| BusUri              | JToken                    | Response        |
| BusUri              | JToken                    | Response        |



### Output


| Property          | Type                      | Description          | Example    
|-------------------|---------------------------|----------------------|----------------------|
| BusUri              | JToken                    | Response        |
| BusUri              | JToken                    | Response        |
| BusUri              | JToken                    | Response        |
| BusUri              | JToken                    | Response        |
| BusUri              | JToken                    | Response        |
| BusUri              | JToken                    | Response        |
| BusUri              | JToken                    | Response        |
| BusUri              | JToken                    | Response        |
| BusUri              | JToken                    | Response        |
| BusUri              | JToken                    | Response        |

# License

This project is licensed under the MIT License - see the LICENSE file for details

# Building

Clone a copy of the repo

`git clone https://github.com/CommunityHiQ/Frends.Amqp.git`

Restore dependencies

dotnet restore

Rebuild the project

dotnet build

Run tests

dotnet test Frends.Amqp.Tests

Create a nuget package

dotnet pack Frends.Amqp

# Contributing
When contributing to this repository, please first discuss the change you wish to make via issue, email, or any other method with the owners of this repository before making a change.

1. Fork the repo on GitHub
2. Clone the project to your own machine
3. Commit changes to your own branch
4. Push your work back up to your fork
5. Submit a Pull request so that we can review your changes

NOTE: Be sure to merge the latest from "upstream" before making a pull request!

# Change Log

| Version             | Changes                 |
| ---------------------| ---------------------|
| 1.0.0 | Initial version of AMQP Tasks. |