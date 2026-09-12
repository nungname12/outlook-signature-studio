# Signature Studio - Native GUI

This is the first native Windows GUI implementation of the Guided Wizard design.

## Build

Run from the project folder in Windows PowerShell:

```powershell
.\build.ps1
```

The portable executable is created at `dist\SignatureStudio.exe`.

## Set Signature

1. Open `SignatureStudio.exe`; the app loads a Starter Draft with sample signature data.
2. Replace the sample name, role, company, phone, mobile, and email with the User's details.
3. Check the New messages / Replies & forwards options.
4. Click `SET SIGNATURE`.
5. Close and reopen Classic Outlook if it was already open.

The app writes only to the signed-in user's `%APPDATA%\Microsoft\Signatures` folder and `HKCU` Office settings. Existing same-name files are copied to `_SignatureStudioBackup` before replacement.

For Outlook 2010, the app also registers the user Signature folder under Office 14.0 so the Signature command and automatic insertion can be recognized after Outlook is restarted.

The generated signature uses `<div>` elements only; it does not use HTML tables, so Outlook should not open the contextual Table Tools ribbon when the signature is selected.

For Outlook 2010 specifically, the app does not write `NewSignature` or `ReplySignature` defaults because external Registry defaults can hide the Signature command. It removes only those two values from the user's Office 14.0 profile, leaves the generated files ready, and lets the user choose the default once in Outlook. Newer Office versions continue to receive automatic defaults as `REG_EXPAND_SZ`.

The app does not modify Ribbon customization or machine-level policy values.

## Compatibility

The first release targets Classic Outlook 2010, 2013, 2016, 2019, 2021 and Microsoft 365 Classic. New Outlook and Outlook Web use a different cloud/settings signature workflow and are not changed by this app.

Logo/image support is intentionally deferred from v1.
