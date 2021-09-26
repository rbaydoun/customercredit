# lastpass-homework

## What is missing
* Connection to database should be secured with TLS.
* REST endpoints should have been secured using HTTPS.
* Unit tests with mocking on some portions of the repositories only.
* Unfortunately didn't have available time to docker-compose a PSQL DB to ship the microservice in a single image.

## Extra documents
Please see LastPass.pdf file for personal notes.

## Instructions
Development was done on OSX.
Testing wsa done using [Postman](https://www.postman.com/).

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
### Initial database configuration:
```
customer_management=# \d customers
                               Table "public.customers"
    Column     |  Type  | Collation | Nullable |                Default
---------------+--------+-----------+----------+---------------------------------------
 id            | bigint |           | not null | nextval('customers_id_seq'::regclass)
 name          | text   |           | not null |
 address       | text   |           | not null |
 date_of_birth | date   |           | not null |
Indexes:
    "customers_pkey" PRIMARY KEY, btree (id)
Referenced by:
    TABLE "cards" CONSTRAINT "cards_customer_id_fkey" FOREIGN KEY (customer_id) REFERENCES customers(id) ON DELETE CASCADE



customer_management=# \d cards
                                Table "public.cards"
   Column    |   Type    | Collation | Nullable |              Default
-------------+-----------+-----------+----------+-----------------------------------
 id          | bigint    |           | not null | nextval('cards_id_seq'::regclass)
 number      | text      |           | not null |
 type        | card_type |           | not null |
 expiry_date | date      |           | not null |
 cvv         | text      |           | not null |
 customer_id | bigint    |           | not null |
Indexes:
    "cards_pkey" PRIMARY KEY, btree (id)
Foreign-key constraints:
    "cards_customer_id_fkey" FOREIGN KEY (customer_id) REFERENCES customers(id) ON DELETE CASCADE



customer_management=# \dT+ card_type
List of data types
-[ RECORD 1 ]-----+------------
Schema            | public
Name              | card_type
Internal name     | card_type
Size              | 4
Elements          | amex       +
                  | visa       +
                  | master_card
Owner             | cm_service
Access privileges |
Description       |



customer_management=# select * from customers;
(0 rows)

customer_management=# select * from cards;
(0 rows)

customer_management=#
```


### Creating some customer profiles
POST /customer
```
curl --location --request POST 'http://localhost:5000/customer' \
--header 'Content-Type: application/json' \
--data-raw '{
    "name": "Reda Baydoun",
    "address": "1234 Test Blv, Quebec, QC",
    "dateOfBirth": "1969-07-21"
}'

>>> Response
{
    "id": 11,
    "name": "Reda Baydoun",
    "address": "1234 Test Blv, Quebec, QC",
    "dateOfBirth": "1969-07-21T00:00:00",
    "cards": []
}

curl --location --request POST 'http://localhost:5000/customer' \
--header 'Content-Type: application/json' \
--data-raw '{
    "name": "John Doe",
    "address": "11 RockyMountain, Montreal, QC",
    "dateOfBirth": "1988-06-04"
}'

>>> Response
{
    "id": 12,
    "name": "John Doe",
    "address": "11 RockyMountain, Montreal, QC",
    "dateOfBirth": "1988-06-04T00:00:00",
    "cards": []
}


curl --location --request POST 'http://localhost:5000/customer' \
--header 'Content-Type: application/json' \
--data-raw '{
    "name": "To Delete",
    "address": "Somewhere on Earth",
    "dateOfBirth": "1990-01-18"
}'
>>> Response
{
    "id": 13,
    "name": "To Delete",
    "address": "Somewhere on Earth",
    "dateOfBirth": "1990-01-18T00:00:00",
    "cards": []
}
```

### Database
```
customer_management=# select * from customers;
-[ RECORD 1 ]-+-------------------------------
id            | 11
name          | Reda Baydoun
address       | 1234 Test Blv, Quebec, QC
date_of_birth | 1969-07-21
-[ RECORD 2 ]-+-------------------------------
id            | 12
name          | John Doe
address       | 11 RockyMountain, Montreal, QC
date_of_birth | 1988-06-04
-[ RECORD 3 ]-+-------------------------------
id            | 13
name          | To Delete
address       | Somewhere on Earth
date_of_birth | 1990-01-18
```


### Retrieve All Customers
GET /customer
```
curl --location --request GET 'http://localhost:5000/customer'

>>> Response
[
    {
        "id": 11,
        "name": "Reda Baydoun",
        "address": "1234 Test Blv, Quebec, QC",
        "dateOfBirth": "1969-07-21T00:00:00",
        "cards": []
    },
    {
        "id": 12,
        "name": "John Doe",
        "address": "11 RockyMountain, Montreal, QC",
        "dateOfBirth": "1988-06-04T00:00:00",
        "cards": []
    },
    {
        "id": 13,
        "name": "To Delete",
        "address": "Somewhere on Earth",
        "dateOfBirth": "1990-01-18T00:00:00",
        "cards": []
    }
]
```

### Retrieve Specific Customer
GET /customer/{id}
```
curl --location --request GET 'http://localhost:5000/customer/11'

>>> Response
{
    "id": 11,
    "name": "Reda Baydoun",
    "address": "1234 Test Blv, Quebec, QC",
    "dateOfBirth": "1969-07-21T00:00:00",
    "cards": []
}
```

### Update Specific Customer
PATCH /customer/{id}
```
curl --location --request PATCH 'http://localhost:5000/customer/11' \
--header 'Content-Type: application/json' \
--data-raw '[
    {
        "value": "Reda R. Baydoun",
        "path": "/Name",
        "op": "replace"
    }
]'

>>> Response
{
    "id": 11,
    "name": "Reda R. Baydoun",
    "address": "1234 Test Blv, Quebec, QC",
    "dateOfBirth": "1969-07-21T00:00:00",
    "cards": []
}

>>> Database
customer_management=# select * from customers where id = 11;
-[ RECORD 1 ]-+--------------------------
id            | 11
name          | Reda R. Baydoun
address       | 1234 Test Blv, Quebec, QC
date_of_birth | 1969-07-21
```

### Delete Specific Customer
DELETE /customer/{id}
```
curl --location --request DELETE 'http://localhost:5000/customer/13'

>>> Response

>>> Database
customer_management=# select * from customers where id = 13;
-[ RECORD 1 ]-+-------------------
id            | 13
name          | To Delete
address       | Somewhere on Earth
date_of_birth | 1990-01-18

< DELETE request happened here>

customer_management=# select * from customers where id = 13;
(0 rows)
```

### Add Credit Card to Customer
POST /customer/{id}/card
```
curl --location --request POST 'http://localhost:5000/customer/11/card' \
--header 'Content-Type: application/json' \
--data-raw '{
    "type": "MasterCard",
    "number": "1111222233334444",
    "expiryDate": "06/2026",
    "cvv": "123"
}'

>>> Response
{
    "id": 3,
    "number": "1111222233334444",
    "type": 2,
    "expiryDate": "2026-06-01T00:00:00",
    "cvv": "123",
    "customerId": 11
}

>>> Database
customer_management=# select * from cards where id = 3;
-[ RECORD 1 ]------------------------------------------------------------------------------------------------------------------------------------------------------------
id          | 3
number      | CfDJ8MlrVR77LuhOswpaIaEMLoscCrqubx8INTlpcDa93GJNq2Od1KJar0z77eMHuyztgtgP__1Ks6z9sCTlwDBAP-oaK5PgG9JIYGHRnSWyi9WBVJfg6mZ48-2u2MPFQ8r9OoiVWEmg1tY0NZGLgZY9YkM
type        | master_card
expiry_date | 2026-06-01
cvv         | CfDJ8MlrVR77LuhOswpaIaEMLouV-VrJbumqYRv4IWJdDTSb08lnlnyzITjq_aNV57-qjdfThzq8FliXcn_LHxOXH0fFBoJMH35LfNFst5nrk7SNemvsesPElD9cHgL-MwE3nw
customer_id | 11




curl --location --request POST 'http://localhost:5000/customer/11/card' \
--header 'Content-Type: application/json' \
--data-raw '{
    "type": "MasterCard",
    "number": "4444333322221111",
    "expiryDate": "06/2026",
    "cvv": "123"
}
'

>>> Response
{
    "id": 4,
    "number": "4444333322221111",
    "type": 2,
    "expiryDate": "2026-06-01T00:00:00",
    "cvv": "123",
    "customerId": 11
}

>>> Database
customer_management=# select * from cards where id = 4;
-[ RECORD 1 ]------------------------------------------------------------------------------------------------------------------------------------------------------------
id          | 4
number      | CfDJ8MlrVR77LuhOswpaIaEMLouM7aHH8ACEFV0ijVp6uAfdnOeVsT9PRrgL7lEdBZzKrYLczP84poII-APpXPoxaEC55PyB4-CWqUkX2ZFD13aiqgW6RivcO15fAHqIJ5V967JgnJC9inK2lSYbAUi1TNg
type        | master_card
expiry_date | 2026-06-01
cvv         | CfDJ8MlrVR77LuhOswpaIaEMLovKDPD6wzQNf-UTEOMMNuKNdKUxkLqCRBaRM3y3B1qGU2obQyWMonpHxAeX4bao52DY6BwgS1S3d3q9GsMZ94mE-K_nTigfxp4QVpoSb2_BWg
customer_id | 11
```

### Update A Credit Card
PATCH /customer/{id}/card/{number}
```
curl --location --request PATCH 'http://localhost:5000/customer/11/card/4444333322221111' \
--header 'Content-Type: application/json' \
--data-raw '[
    {
        "value": "Amex",
        "path": "/Type",
        "op": "replace"
    }
]'

>>> Response
{
    "id": 4,
    "number": "4444333322221111",
    "type": 0,
    "expiryDate": "2026-06-01T00:00:00",
    "cvv": "123",
    "customerId": 11
}


>>> Database
customer_management=# select * from cards where id = 4;
-[ RECORD 1 ]------------------------------------------------------------------------------------------------------------------------------------------------------------
id          | 4
number      | CfDJ8MlrVR77LuhOswpaIaEMLosaYb9owSPXO_-Qo18SblVe4CbMwDj-Dcz35XNmcQRjUJlE4djJiU-RwCiRUA2kYWX7AhB9TZMFpexJIzWVHCal0sIV3_KZX0N3fI_-2J-LR_TrvZQAJM2Tn0DyGRDqpjA
type        | amex
expiry_date | 2026-06-01
cvv         | CfDJ8MlrVR77LuhOswpaIaEMLov9hw5BcpDXCPHwMJer-HnDQQfNEO_hSg_GBdkHC6Gqtjq7GFU0YfVG62TfjSPqGM-R6NK7wMMjltGeVvNASnOQY9jYFhYNdoPz9driAA1Mrg
customer_id | 11
```

### Delete a Card
DELETE /customer/{id}/card/{number}
```
curl --location --request DELETE 'http://localhost:5000/customer/11/card/4444333322221111'

>>> Response

>>> Database
customer_management=# select * from cards where id = 4;
(0 rows)
```

### Validate Ownership
GET /customer/{id}/card

Success
```
curl --location --request GET 'http://localhost:5000/customer/11/card/' \
--header 'Content-Type: application/json' \
--data-raw '{
    "number": "1111222233334444",
    "type": "MasterCard",
    "expiryDate": "06/2026",
    "cvv": "123"
}'

>>> Response
true

NOTE: This is card id 3 in the database
customer_management=# select * from cards where id = 3;
-[ RECORD 1 ]------------------------------------------------------------------------------------------------------------------------------------------------------------
id          | 3
number      | CfDJ8MlrVR77LuhOswpaIaEMLoscCrqubx8INTlpcDa93GJNq2Od1KJar0z77eMHuyztgtgP__1Ks6z9sCTlwDBAP-oaK5PgG9JIYGHRnSWyi9WBVJfg6mZ48-2u2MPFQ8r9OoiVWEmg1tY0NZGLgZY9YkM
type        | master_card
expiry_date | 2026-06-01
cvv         | CfDJ8MlrVR77LuhOswpaIaEMLouV-VrJbumqYRv4IWJdDTSb08lnlnyzITjq_aNV57-qjdfThzq8FliXcn_LHxOXH0fFBoJMH35LfNFst5nrk7SNemvsesPElD9cHgL-MwE3nw
customer_id | 11
```


Fail -- Changed the CVV from 123 to 333 > This showcases that it validates the encrypted fields in the database.
```
curl --location --request GET 'http://localhost:5000/customer/11/card/' \
--header 'Content-Type: application/json' \
--data-raw '{
    "number": "1111222233334444",
    "type": "MasterCard",
    "expiryDate": "06/2026",
    "cvv": "333"
}'

>>> Response
false
```


### Bonus -- Delete Customer with Associated Cards.
```
customer_management=# select * from customers where id = 11;
-[ RECORD 1 ]-+--------------------------
id            | 11
name          | Reda R. Baydoun
address       | 1234 Test Blv, Quebec, QC
date_of_birth | 1969-07-21

customer_management=# select * from cards where customer_id = 11;
-[ RECORD 1 ]------------------------------------------------------------------------------------------------------------------------------------------------------------
id          | 3
number      | CfDJ8MlrVR77LuhOswpaIaEMLoscCrqubx8INTlpcDa93GJNq2Od1KJar0z77eMHuyztgtgP__1Ks6z9sCTlwDBAP-oaK5PgG9JIYGHRnSWyi9WBVJfg6mZ48-2u2MPFQ8r9OoiVWEmg1tY0NZGLgZY9YkM
type        | master_card
expiry_date | 2026-06-01
cvv         | CfDJ8MlrVR77LuhOswpaIaEMLouV-VrJbumqYRv4IWJdDTSb08lnlnyzITjq_aNV57-qjdfThzq8FliXcn_LHxOXH0fFBoJMH35LfNFst5nrk7SNemvsesPElD9cHgL-MwE3nw
customer_id | 11

curl --location --request DELETE 'http://localhost:5000/customer/11/'

>>> Response


>>> Database
customer_management=# select * from customers where id = 11;
(0 rows)

customer_management=# select * from cards where customer_id = 11;
(0 rows)
```