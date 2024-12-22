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

The **vBase Core** library implements core client functionality.
It consists of the following main classes and interfaces:

- **ICommunicationChannel**: Responsible for delivering messages from the client to a vBase Contract deployed on the Ethereum blockchain.  
  This class does not know about specific contract methods; it only knows how to call methods by their names and parameters.

- **CommitmentService**: Mimics the vBase Smart Contract Interface. With the help of ICommunicationChannel, this class allows the execution of the contract's methods.

- **vBaseDataset**: Records data on the blockchain with the assistance of the vBase Smart Contract. It also enables validation of the recorded data.  
  Validation, in this context, ensures that exactly the same data records were submitted with the specified timestamps and that no other records were submitted within the scope of this dataset.

![](https://img.plantuml.biz/plantuml/png/TP51QiCm44NtEiNWPI0zGIXfvPfaLt0kC9KdjQYaCT8uZg67h-JG9Wra8pxC-_-Ff6qIZ39wfvHnc19KchstuYb8I_5a3LM02LfbWr0yeY6ezeKPWpKebeFkCGHo2wRrRqp3SSBOKNp8DbTu9p8yv7PNxlIAYRIpURbBtRz1ZP9FsHoAECm3FIzGhLGBg_MwBASrRBwpR6vqoTSBqAhw0YeLM0YY_Um5-9W70E-HppC8mz85oUTt0yD18XfvKXGwAAPxQmu7U76EfrJCvk-M19EL7l0kATCT3OvdwWh_9Zr56ZryE3PVfaSRRhksMPRF-m40)
