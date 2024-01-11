# Security Policy

To mitigate the risks and ensure the security of this set of libraries, 
all submitted pull requests must be checked for vulnerabilities using code scanning tools: 
[Sonarqube][3] and [CodeQL][4].
If the PR contains vulnerabilities with severity is greater than or equal middle severity, then the PR cannot be merged.

## Supported Versions

| Version | Supported          |
| ------- | ------------------ |
| 8.x.x   | :white_check_mark: |
| 6.x.x   | :x:                |
| 5.x.x   | :x:                |
| 3.1.x   | :x:                |
| < 3.1   | :x:                |

## Reporting a Vulnerability

Security issues and bugs should be reported privately, via email, 
to the Raiffeisen .NET Community through by emailing [DotNetCommunity@raiffeisen.ru][1].
Please, specify **\[SEC\] Reporting a Vulnerability** on the message Subject.
You should receive a response within 48 hours.

>❗ **IMPORTANT: Please do not open issues on the [repository issue tracker][2] for anything you think might have a security implication.**

[1]: <mailto:DotNetCommunity@raiffeisen.ru?subject=\[Sec\]%20Reporting%20a%20Vulnerability> "E-mail Raiffeisen .NET Community"
[2]: <https://github.com/Raiffeisen-DGTL/ViennaNET/issues> "ViennaNET issue tracker"
[3]: <https://sonarcloud.io/dashboard?id=Raiffeisen-DGTL_ViennaNET> "ViennaNET sonarqube project"
[4]: <https://github.com/Raiffeisen-DGTL/ViennaNET/actions/workflows/codeql-analysis.yml> "CodeQL workflow"
