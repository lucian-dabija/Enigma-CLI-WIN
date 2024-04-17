# Enigma-CLI SSH Module for Windows
Enigma-CLI SSH Module for Windows is a simple, yet powerful tool designed to ease the process of using SSH from Windows to Linux systems. It's a common scenario where non-technical staff continue to use password authentication due to the absence of ssh-copy-id on Windows and the complexity of manually copying public keys or generating their first key pair. This tool aims to abstract these complexities, making it easier for everyone to use Public Key Infrastructure (PKI) in a company's infrastructure.

## Importance
The use of PKI for SSH authentication significantly enhances the security of remote connections. However, the process of setting up PKI can be daunting for non-technical staff. They often resort to using password authentication, which is less secure and does not leverage the full potential of SSH.
Enigma-CLI SSH Module for Windows fills this gap by providing a simple and straightforward way to set up PKI for SSH. It automates the process of generating a key pair, copying the public key to the remote server, and adding it to the authorized_keys file. This eliminates the need for manual intervention and makes it easy for anyone to start using PKI for SSH.

## Features
•	Automated Key Pair Generation: If a public key is not found on the local system, the tool prompts the user to generate a new key pair using ssh-keygen.
•	Automated Public Key Copying: The tool automatically copies the public key to the remote server and appends it to the authorized_keys file.
•	Password Masking: When prompting for a password, the tool masks the input with asterisks to prevent it from being displayed on the screen.
•	Error Handling: The tool provides informative error messages if anything goes wrong during the SSH connection or command execution.

## Deployment
Enigma-CLI SSH Module for Windows can be easily deployed across a company's infrastructure using Group Policy Objects (GPO) or any other deployment method. This ensures that everyone in the company has access to this tool and can start using PKI for SSH without any technical hurdles.

## Conclusion
Enigma-CLI SSH Module for Windows is a game-changer for companies that want to leverage the power of PKI for SSH. It simplifies the process of setting up PKI, making it accessible to everyone, regardless of their technical expertise. By automating the complex parts of the process, it allows everyone to focus on their work without worrying about the intricacies of SSH and PKI.