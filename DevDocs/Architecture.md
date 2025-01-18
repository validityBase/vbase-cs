# Abstractions

The software consists of the following components:  

- **Ethereum Blockchain**: Provides verifiable provenance, establishing the credibility of the data.  
- **Validity Base Forwarder API**: A RESTful API that forwards data to the blockchain on behalf of the client.  
- **Validity Base Client**: A client application that interacts with the Forwarder API to send data to the blockchain.  

```plantuml(Abstraction Diagram)
@startuml

[Client] #palegreen

cloud {
  [Forwarder]
  [Ethereum]
  [vBase Smart Contract]
}


[Client] --> [Forwarder] : HTTP
[Forwarder] --> [Ethereum]
[Ethereum] o-- [vBase Smart Contract]

@enduml
```

## Client

The client part consists of the following components:  

- **vBase Core**: The core library that implements communication with the vBase Forwarder API.  
  It is written in .NET Standard 2.0, which makes it compatible with both .NET Core and .NET Framework.  
- **vBase COM Shim**: A COM shim that enables the **vBase Core** library to be used in a COM environment, such as Visual Basic for Applications.  

```plantuml(Client Diagram)
@startuml

node "Client Environment" {

    package "vBase SDK" {
        [vBase.Core] as C #palegreen
        [vBase COM Shim] as S #palegreen
    }   

    [Execl VBA] as VBA #LightGray
    [.Net Framework Clients] as NF #LightGray
    [.Net Core Clients] as NC #LightGray
}

note bottom of C
  .Net Standard 2.0 compatible with
  .Net Core and .Net Framework. 
end note

note bottom of S
  .Net Framework library
  exposing COM interface 
end note

[VBA] --> [S]
[NF] --> [C]
[NC] --> [C]

@enduml
```

### vBase Core

The **vBase Core** library implements core vBase client functionality.
It consists of the following main classes and interfaces:

- **ICommitmentService**: Represents base commitment operations.
- **Web3CommitmentService**: Ethereum blockchain commitment service, based on the vBase Smart Contract.
  This class is abstract because it does not define the actual implementation for delivering messages to the Smart Contract.
- **ForwarderCommitmentService**: A vBase Smart Contract commitment service where messages are delivered via the vBase Forwarder.
- **vBaseDataset**: Represents a records set on the blockchain, commited with ICommitmentService.
  It also facilitates validation of the commoted recorded data, ensuring that:
  - Exactly the same data records were commited with the specified timestamps.
  - No additional records were commited within the scope of the dataset.
  - All the records in the dataset were commited.

```plantuml(vBase Core Class Diagram)
@startuml

interface "ICommitmentService" as CS
abstract class "Web3CommitmentService" as W3CS
class "ForwarderCommitmentService" as FCS
class "HttpCommitmentService" as HCS #dadada ##[dotted]
class "vBaseClient" as C
class "vBaseDataset" as D

abstract class "vBaseObject" as VBO
class "vBaseStringObject" as VBO_S

CS <|-- W3CS
W3CS <|-- FCS
W3CS <|-- HCS
D "1" *-- "1" C
C *-- CS
D "1" *-- "many" VBO

VBO <|-- VBO_S

note left of CS
  Represents base commitment operations.
end note

note left of W3CS
  Ethereum blockchain commitment service,
  based on the vBase Smart Contract.
  This class is abstract because it
  does not define the actual implementation
  for delivering messages to the Smart Contract.
end note


@enduml

```