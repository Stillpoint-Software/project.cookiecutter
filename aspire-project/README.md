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

#### Database

The Aspire will create the database, however, you will need to create the tables and columns.  If you are using **Postgesql** and need auditing, you will need to add the **pgcrypto** extension to the correct schema when running locally in order to encrypt the data.  You can to this by adding the extension in the database\extension folder and look for **pgcrypto**.  Make sure the definition of the extension is **public**;

Also, give public access to run the **pgp_sym_encrypt** function.

`GRANT EXECUTE ON FUNCTION pgp_sym_encrypt(text, text) TO {Your DB username from Aspire}}`