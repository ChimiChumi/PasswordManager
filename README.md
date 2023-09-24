# Password Manager

## Description
This is a console-based password manager written for a University project. The program stores the user and their encrypted login credentials in a local .csv file.
To use it, first register a new user and set a master-password. After this is done, you can start saving the login details for specific sites. 
Everything works straight from the terminal using command-line arguments.

## Help
Here's a list of the possible command line arguments. The ones written in ** < > ** are mandatory, whereas the others are optional
- `--workdir <path>` : sets the path of the directory where the .csv files are kept
- `--register <username> <email> <master-password> [firstname] [lastname]>`: creates a new user for the password manager
- `--login <username>`: after the user name was given you are prompted to enter the password. If it's correct, you are successfully signed in and have access for further commands:
		- `--list`: TODO
		- `--add`: TODO
		- `--delete`: TODO
		- `--logout`: TODO