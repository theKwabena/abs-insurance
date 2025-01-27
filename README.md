
# Project Documentation

## Overview
This project implements a premium calculation system where premiums can be added or subtracted based on the market value and component settings.

## Setup Instructions

### 1. Clone the Repository

Start by cloning the repository to your local machine:

```bash
git clone https://github.com/theKwabena/abs-insurance
cd abs-insurance
```

### 2. Environment Setup

Before running the project, you need to configure the environment variables. Follow these steps:

- Copy the `.env.example` file to `.env`:

```bash
cp .env.example .env
```

- Open the `.env` file and provide the necessary values:

    - `POSTGRESUSER`: The username for your PostgreSQL database.
    - `POSTGRES_PASSWORD`: The password for your PostgreSQL user.
    - `POSTGRES_CONNECTION_STRING`: The connection string for the PostgreSQL database.
    - `ADMIN_CREATION_TOKEN`: The token used for creating admin tokens.
    - `SECRET`: The secret key for JWT authentication.

Example `.env`:

```env
POSTGRES_USER=YOUR-PG-USER
POSTGRES_PASSWORD=YOUR-PG-PASSWORD
ADMIN_CREATION_TOKEN=YOUR-CREATION-TOKEN-FOR-CREATING-ADMIN-USERS
SECRET=YOUR-JWT-SECRET
POSTGRES_CONNECTION_STRING=YOUR-PG-CONNECTION-STRING
 
```

### 3. Running the Project

To run the project using Docker, you need to execute the following command:

```bash
docker-compose -f docker-compose.local.yaml up
```

This will start the necessary services defined in the `docker-compose` file (`my.yaml`), such as the PostgreSQL database and the application container.

### 4. Accessing the Application

Once the containers are up and running, you can access the application by navigating to `http://localhost:8080/swagger/index.html` in your web browser to access swagger url

## Environment Variables

Here’s a description of the environment variables used by the application:

- **PostgresUser**: Specifies the PostgreSQL username to be used for database connections.
- **PostgresPassword**: Specifies the password associated with the PostgreSQL user.
- **PgConnectionString**: The connection string used for connecting to the PostgreSQL database.
- **Admin-Secret**: A token used to generate admin tokens for secure access.
- **secret**: A secret key used for JWT authentication.

## Database Schema

The project uses PostgreSQL to store its data. Ensure that your PostgreSQL container is running and accessible via the connection string provided in the `.env` file.

### Tables
- **Policy**: Stores premium data, including market value, premium type, and calculation results.
- **PolicyComponents**: Stores different components like `MarketValuePremium`, with fields for percentage value and flat value.



## Troubleshooting

- If the Docker containers fail to start, make sure that your `.env` file contains the correct PostgreSQL credentials.
- Ensure that the port for the PostgreSQL database is available and not blocked by any firewall.
