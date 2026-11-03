# Code signing policy

## Current status

The SignPath integration is prepared in the `release/0.1.0-pre.15` branch, but the SPECTRA project has not yet been provisioned with a SignPath Foundation certificate. No SPECTRA release should be described as signed until a release has completed both signing requests and its Authenticode signatures have been verified.

The intended provider is **Free code signing provided by SignPath.io, certificate by SignPath Foundation**. This is pending project acceptance and certificate provisioning; it is not a statement that a certificate has already been issued.

## Roles and approvals

- **Committers and reviewers:** SPECTRA maintainers with write access to the [SPECTRA repository](https://github.com/femboy-organization/SPECTRA). Repository access is managed through GitHub.
- **Signing approvers:** owners of the [femboy-organization GitHub organization](https://github.com/orgs/femboy-organization/people?query=role%3Aowner), provided they have access to the SignPath project. Each release signing request must receive manual approval from an authorized approver.
- All people with repository write access or signing permissions must use multi-factor authentication on both GitHub and SignPath.

Changes to the release workflow, signing configuration, and build scripts require the same review as application source changes. A release is not eligible for signing merely because a workflow run succeeded.

## What is signed

The SignPath artifact configurations are intentionally limited to SPECTRA-owned binaries:

- The top-level `SPECTRA.exe` launcher.
- `bin/SPECTRA.exe`, `bin/SPECTRA.dll`, and `bin/SPECTRA.Core.dll`.
- The SPECTRA Windows installer executable.

The project does not sign upstream or third-party binaries with a SPECTRA/SignPath Foundation certificate. This includes the bundled scrcpy and ADB tools, .NET runtime files, and third-party libraries. Their own publisher signatures are retained where supplied. The installer’s generated uninstaller is not currently covered by the post-build signing configuration and must not be represented as signed.

## Release verification

Release builds are blocked unless the required SignPath configuration is available. The workflow submits the application payload and the installer as separate signing requests, waits for completion, restores the signed files, and verifies their Authenticode signatures and product metadata before generating checksums and publishing the release.

A valid Authenticode signature is not a blanket guarantee that Windows Smart App Control will allow every bundled upstream executable. Smart App Control also evaluates binaries independently; unsigned third-party tools may still be subject to Windows policy.

## Privacy

This program will not transfer any information to other networked systems unless specifically requested by the user or the person installing or operating it. SPECTRA uses ADB and scrcpy to communicate with a Meta Quest device when the user initiates device discovery, pairing, connection, or streaming. SPECTRA does not intentionally upload headset content or diagnostic logs to a SPECTRA-operated remote service.

## SignPath setup required

After SignPath Foundation accepts the project and provisions the certificate:

1. Enable MFA for every repository committer/reviewer and signing approver.
2. In SignPath, create the SPECTRA project with repository URL `https://github.com/femboy-organization/SPECTRA`, link the GitHub.com trusted build system, and install/authorize the SignPath GitHub App as required by SignPath.
3. Import the two XML artifact configurations from `build/signpath/`.
4. Create a release signing policy that uses the SignPath Foundation certificate, requires manual approval, and restricts origin verification to the protected release source (the `main` branch and/or `release/*` branches).
5. Add these GitHub repository variables:
   - `SIGNPATH_ORGANIZATION_ID`
   - `SIGNPATH_PROJECT_SLUG`
   - `SIGNPATH_SIGNING_POLICY_SLUG`
   - `SIGNPATH_PORTABLE_ARTIFACT_CONFIGURATION_SLUG`
   - `SIGNPATH_INSTALLER_ARTIFACT_CONFIGURATION_SLUG`
6. Add the SignPath API token as the GitHub Actions secret `SIGNPATH_API_TOKEN`. Never commit the token or a certificate/private key to the repository.
7. Run the workflow manually on `release/0.1.0-pre.15`. Confirm that both requests were approved and that the published release assets contain valid signatures.

The release workflow deliberately fails closed when any of the required signing settings are missing. It must not publish an unsigned release as though signing had succeeded.
