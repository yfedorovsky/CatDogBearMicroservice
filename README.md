# CatDogBearMicroservice

## Overview
The CatDogBearMicroservice is a C# microservice that fetches random pictures of cats, dogs, or bears from specified APIs. It provides a REST API for saving and retrieving pictures, making it easy to manage and access animal images.

## Features
- Fetch random pictures of cats, dogs, or bears.
- Save pictures to a database.
- Retrieve the last stored picture of a specified animal type.

## Project Structure
```
CatDogBearMicroservice
├── src
│   ├── Controllers
│   │   └── PicturesController.cs
│   ├── Models
│   │   └── Picture.cs
│   │   └── PictureDbContext.cs
│   ├── Services
│   │   └── PictureService.cs
│   ├── CatDogBearMicroservice.csproj
│   └── Program.cs
├── Dockerfile
├── .dockerignore
├── .gitignore
└── README.md
```

## Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- Docker (for containerization)

### Running the Microservice

1. Clone the repository:
   ```sh
   git clone <repository-url>
   cd CatDogBearMicroservice
   ```

2. Restore the dependencies:
   ```sh
   cd src
   dotnet restore
   ```

3. Run the application:
   ```sh
   dotnet run
   ```

4. The microservice will be available at `http://localhost:8080`.

### Using Docker

To build and run the microservice using Docker, follow these steps:

1. Build the Docker image:
   ```sh
   docker build -t catdogbearmicroservice .
   ```

2. Run the Docker container:
   ```sh
   docker run -d -p 8080:80 --name catdogbearmicroservice catdogbearmicroservice
   ```

3. Access the microservice at `http://localhost:8080`.

### API Endpoints

- **Save Pictures**

  ```sh
  POST /api/pictures/save?animalType={animalType}&numberOfPictures={numberOfPictures}
  ```

  Example:

  ```sh
  curl -X POST "http://localhost:8080/api/pictures/save?animalType=cat&numberOfPictures=1"
  ```

- **Get Last Picture**

  ```sh
  GET /api/pictures/last?animalType={animalType}
  ```

  Example:

  ```sh
  curl -X GET "http://localhost:8080/api/pictures/last?animalType=cat"
  ```

### Logging

Logs are output to the console. You can view the logs of the running Docker container using:

```sh
docker logs catdogbearmicroservice
```

### Stopping and Removing the Docker Container

To stop the running container:

```sh
docker stop catdogbearmicroservice
```

To remove the container:

```sh
docker rm catdogbearmicroservice
```

## Contributing
Contributions are welcome! Please open an issue or submit a pull request for any improvements or bug fixes.

## License
This project is licensed under the MIT License. See the LICENSE file for details.