import sys
import subprocess
import os
import shutil

def main():
    # Debug: show all arguments
    print(f"🔎 sys.argv: {sys.argv}")

    # Retrieve variables from command-line arguments
    if len(sys.argv) < 6:
        print("Error: Missing required arguments.")
        sys.exit(1)

    environment = sys.argv[1]
    assembly_name = sys.argv[2]
    database = sys.argv[3] 
    project_path = sys.argv[4]
    template_path = sys.argv[5]

    print(f"✅ Parsed args -> Environment: {environment}, Assembly: {assembly_name}, Database: {database}")
    print(f"   Project Path: {project_path}")
    print(f"   Template Path: {template_path}")

    # Run azd login
    try:
        subprocess.run(
            ["azd", "auth", "login", "--scope", "https://management.azure.com//.default"], 
            check=True
        )
        print("Successfully logged in using 'azd login'")
    except subprocess.CalledProcessError as e:
        print(f"Error running 'azd login': {e}")
        sys.exit(1)
       
    # Run azd init
    try:
        subprocess.run(["azd", "init","-e", environment, "--cwd", project_path ], check=True)
        print("Successfully ran 'azd init'")
    except subprocess.CalledProcessError as e:
        print(f"Error running 'azd init': {e}")
        sys.exit(1)

    # If using MongoDB, run infra synth and copy the bicep file
    if database == "MongoDb":
        try:
            subprocess.run(["azd", "infra", "synth"], check=True)
            print("Successfully ran 'azd infra synth'")
        except subprocess.CalledProcessError as e:
            print(f"Error running 'azd infra synth': {e}")
            sys.exit(1)

        # Copy mongodb.module.bicep
        mongodb_bicep = os.path.join(template_path, 'mongodb.module.bicep')
        infra_folder_path = os.path.join('infra', 'mongodb')

        if os.path.isfile(mongodb_bicep):
            if not os.path.exists(infra_folder_path):
                os.makedirs(infra_folder_path)
            shutil.copy(mongodb_bicep, infra_folder_path)
            print(f"Copied '{mongodb_bicep}' to '{infra_folder_path}'")
        else:
            print(f"File '{mongodb_bicep}' does not exist.")
            sys.exit(1)

        # Run azd pipeline config
        try:
            subprocess.run(["azd", "pipeline","config", "-e", environment], check=True)
            print("Successfully ran 'azd github setup for environment'")
        except subprocess.CalledProcessError as e:
            print(f"Error running 'azd github setup' for environment '{environment}': {e}")
            sys.exit(1)        

if __name__ == "__main__":
    main()
