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

![vBase Core Class Diagram](https://img.plantuml.biz/plantuml/png/TP9DImCn48Rl2_iVXkrDR45wzL2wjTHJmHLx495iCjj6yx4aquhWZpSVPMbjPI7P8SyxyyuaCq_CqLwhiYWBQGXTnpX2zLXRhILfDDIWEqYE5J0FTLCMhFNa62VWYdaFrGRRwNLwCuty4LjQzyMSG7UTNPwWAwB-EhMg6nW95ZyOZLw59KBnDWWFS-QnLZAeijtpamMZiEQZHMpuRok9MhSVo3FqCb__0pNafDcUe-zDp1aStl-CnyVcuvhZvNcuYk42ghiARa8Oz-2sJi6V8ytCTvMzb4NOSeAXeB64eB0Zi5sI0ZnXxz27ANXeWrdWmp31zkWOIMlyf2pG28ZoYpJPFC03xT3XNaEhBF_aEoRDQJAVh-Oseh6G06iWI23D21eTtXRKrgJvJIBrl9F-ECZmCuosHSxsGI4fGiAYZxv0O2SDfeo1sZC5KlSAO-tKGuGxwmAdv07ZZO16xzaswCactOM7qvxBOXR2zFf_0G00)
