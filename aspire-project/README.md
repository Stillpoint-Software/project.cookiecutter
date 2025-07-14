# aspire project.cookiecutter

**aspire project.cookiecutter** provides a streamlined process to setup a web API project with support for *OAuth*, *Azure*, and *Auditing* for **Aspire**.  

If there are any updates to this template, there is a github process that will create a new branch, update the project, run tests and create a pull request.

## Solution Structure

The solution consists of the following projects:

- **API**
- **Abstractions**
- **AppHost**
- **Database**
- **Migrations**
- **ServiceDefaults**
- **Tests**

##### Databases
- **Postgesql**
    The Aspire will create the database, however, you will need to create the tables and columns.  If you are using **Postgesql** and need auditing, you will need to add the **pgcrypto** extension to the correct schema when running locally in order to encrypt the data.  You can to this by adding the extension in the database\extension folder and look for **pgcrypto**.  Make sure the definition of the extension is **public**.  Also, give public access to run the **pgp_sym_encrypt** function.

    `CREATE EXTENSION IF NOT EXISTS pgcrypto;`

    `GRANT EXECUTE ON FUNCTION pgp_sym_encrypt(text, text) TO {Your DB username from Aspire}}`


   `CREATE OR REPLACE FUNCTION db_sym_encrypt(input_text TEXT, key TEXT)
RETURNS BYTEA AS $$
BEGIN
    -- Replace this with your encryption logic
    RETURN pgp_sym_encrypt(input_text, key);
END;
$$ LANGUAGE plpgsql;`

`CREATE OR REPLACE FUNCTION db_sym_decrypt(input_cipher BYTEA, key TEXT)
RETURNS TEXT AS $$
BEGIN
    RETURN pgp_sym_decrypt(input_cipher, key);
END;
$$ LANGUAGE plpgsql;`


Here is script for audit table


CREATE TABLE IF NOT EXISTS samplemessages.audit_event
(
    event_id SERIAL PRIMARY KEY,
    event_type TEXT NOT NULL,
    data jsonb,
    last_updated TIMESTAMP WITH TIME ZONE NOT NULL
)




Here is the script for the sample data

CREATE SCHEMA IF NOT EXISTS samplemessages

CREATE TABLE IF NOT EXISTS samplemessages.sample
(
    id SERIAL PRIMARY KEY,
    name TEXT NOT NULL,
    description bytea,
    created_by TEXT NOT NULL,
    created_date TIMESTAMP WITH TIME ZONE NOT NULL
)


- **MongoDb**
  Aspire does not automatically create the Database.  You will need to create the database and tables separately.
 