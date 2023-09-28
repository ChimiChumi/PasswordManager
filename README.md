# Password Manager

## Description
This is a console-based password manager written for a University project. The program stores the user and their encrypted login credentials in a local .csv file.
To use it, first register a new user and set a master-password. After this is done, you can start saving the login details for specific sites. 
Everything works straight from the terminal using command-line arguments.

## Guide
You can find a demo user under **`resources/demo.txt`**. Here's a list of all the possible command line arguments:
- `--workdir <path>`: sets the path of the directory where you want the .csv files to be created
- `--register`: prompts you to enter details. Upon success, creates a new user for the password manager
- `--login`: prompts you to authenticate. Upon success, you have access to your vault. The session will be active until you log out. Further further commands:
	- `list`: shows all the saved secrets in a decrypted way
	- `add`: possibility to save a new secret
	- `delete`: option to delete a selected entry 
	- `exit`: closes the session by logging out the user
