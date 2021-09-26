# lastpass-homework

## What is missing
* Connection to database should be secured with TLS.
* REST endpoints should have been secured using HTTPS.
* Unfortunately didn't have available time to docker-compose a PSQL DB to ship the microservice in a single image.

## Extra documents
Please see LastPass.pdf file for personal notes.

## Instructions
Development was done on OSX.

### Setting up the database
Postgres 13 was used. See instructions [here](https://postgresapp.com/).

Here is the migration script.
```
-- DB Migration
CREATE USER cm_service WITH ENCRYPTED PASSWORD 'ThisIsForTheLastPassHomework2021';
CREATE DATABASE customer_management;
ALTER DATABASE customer_management OWNER TO cm_service;
GRANT ALL PRIVILEGES ON DATABASE customer_management TO cm_service;

-- Create tables
CREATE TYPE card_type AS ENUM ('amex', 'visa', 'master_card');
ALTER TYPE card_type OWNER TO cm_service;

CREATE TABLE IF NOT EXISTS customers
(
  id bigserial NOT NULL,
  name text NOT NULL,
  address text NOT NULL,
  date_of_birth date NOT NULL,
  
  PRIMARY KEY (id)
);
ALTER TABLE customers OWNER TO cm_service;

CREATE TABLE IF NOT EXISTS cards
(
  id bigserial NOT NULL,
  number text NOT NULL,
  type card_type NOT NULL,
  expiry_date date NOT NULL,
  cvv text NOT NULL,
  customer_id bigint NOT NULL references customers(id) on delete cascade,
  PRIMARY KEY (id)
);
ALTER TABLE cards OWNER TO cm_service;
```

## Proof of work
