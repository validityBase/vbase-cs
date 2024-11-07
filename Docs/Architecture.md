# Abstractions

The software consists of the following components:  

- **Ethereum Blockchain**: Provides verifiable provenance, establishing the credibility of the data.  
- **Validity Base Forwarder API**: A RESTful API that forwards data to the blockchain on behalf of the client.  
- **Validity Base Client**: A client application that interacts with the Forwarder API to send data to the blockchain.  

![Abstraction Diagram](https://img.plantuml.biz/plantuml/png/SoWkIImgAStDuUNYvOfspibCpIk9LL0kICn9JIzAJSq32IVdv9UcA5JpSYaeHBlb5vKd5gMa5iM2kQub6Qb5gQMv45wPKs9nga9mBj141UVyl9AYnEGIe4mjo10aqtLrxP0DKh1Iy0W92G0gG183gq4o7S5MoDVLnMaLBvT3QbuAq3i0)


## Client

The client part consists of the following components:  

- **vBase Core**: The core library that implements communication with the vBase Forwarder API.  
  It is written in .NET Standard 2.0, which makes it compatible with both .NET Core and .NET Framework.  
- **vBase COM Shim**: A COM shim that enables the **vBase Core** library to be used in a COM environment, such as Visual Basic for Applications.  

![Client Diagram](https://img.plantuml.biz/plantuml/png/TPBDJiCm383lbVeErdPUXNW0D1PM3a0vLE9IxU2sNXijFrLgjY6qToVDoc0RM8eIsvz_bNdFwFXTgpYA8sDhWebGaWp3qcobiqRxzmG-1pTuwR3QOEEfxG9xWlpXAJXXb2AO4s4ThG1x433jK57ZYCmb1UBr1V9Mwa3cL-J1d--onN9FbOAtnNs0_GtJPzcq_EZmOqIIZ1XIXvfsrctWE4OV-2pz1nyQFIV56NauheK9IijiDTWrY251YCuPJOskXjla1YghEsHPAeATvFO4XHAUW_F-4ZyRUUkG_8yY-Id-Po8bIuSkz4_xplRID667qZ2vDLPqaR88wvhDxg38LrxDqb4JGHE_j3YQ-qZ_3Ru0)


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

![vBase Core Class Diagram](https://img.plantuml.biz/plantuml/png/RPB1QiCm38RlWRo3I6zZBOnUToXDLxfJOHdhOOpXs4hhBRQ3hNGCzl2JanAQjWJi27-_z4ls8f3mr9ewJT94Mq9V2OcGFUVE64q6BHNeZrfY1Y90NgI9A0Dv8GbaBKA0R8Vb_3QzcqV-XAsT_n5UeR_Dhi_G3L5pczha1KoKY1zC9k_A4Q7w68J7fGYOrvfLlTtnoKeGh_tHAZPysKf7RSilb3tqjjoECnJajTsFYSyYfc9ZZt_JwQddkFRnUXnkOhY2x263EmxZpYRpBhWuCiB-Phs5DE6jJp0Kj8uGQgm8NDL90LwmyHYu-G0bcmKvp11SWrwGTZRCqWIjWYY_IjER1tYY0tfi3PIraz_o8BGzJnRw6xcFQ2oam5bW2NGpWiBmauBSsMwsiqYz7dGu3Pb_Xh6NA4NB2aqHKWv3z0KAAssnoyXKAshGfganrkvwY73bF7ErFcAy4J0OWjYpdbodk_9mtdEQB3ZaH_yF)
