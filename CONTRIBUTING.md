# Contributing to CruxIT

Thanks for your interest in improving CruxIT. Contributions should keep the libraries focused, compatible with the supported target framework, and straightforward to use.

## Before you start

- Search existing issues and pull requests to avoid duplicate work.
- For a significant change, open an issue first to discuss the proposed API or behavior.
- Do not include secrets, credentials, private customer data, or unrelated generated files.

## Make a change

1. Fork the repository and create a branch for your change.
2. Keep the change focused and follow the surrounding C# style.
3. Update the README or other documentation when public behavior or setup changes.
4. Build the solution with the .NET 10 SDK:

   ```sh
   dotnet restore CruxIT.sln
   dotnet build CruxIT.sln --configuration Release --no-restore
   ```

5. If behavior changes, include a clear manual verification description or add/update automated coverage when a test project is available.
6. Open a pull request describing the problem, the change, and how you verified it.

## Pull request expectations

- Keep public API changes intentional and explain compatibility impacts.
- Link any related issue.
- Make sure the proposed change does not include build output, package files, or local IDE state.
- Do not publish a package from a pull request. Releases are published from version tags by the repository workflow.
