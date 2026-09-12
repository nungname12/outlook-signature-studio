# Outlook Signature Compatibility Research

## Decision

Implement the first Outlook integration path for **Classic Outlook for Windows 2010 and later**. The app will generate an Outlook-compatible HTML signature package and a PowerShell installer that writes to the current user's signature directory, creates a backup, and optionally sets the default signature values.

New Outlook for Windows and Outlook on the web remain a separate path because Microsoft documents their signature editor and settings separately from Classic Outlook.

## Evidence

- Microsoft documents that Classic Outlook signatures are created from a new message via **Signature > Signatures**, can include text, images and links, and can be selected separately for new messages and replies/forwards: [Create an email signature in Outlook](https://support.microsoft.com/en-us/topic/create-an-email-signature-in-outlook-0f6ec33a-94dc-c87b-ad05-3bd04e89f51c) and [Create an email signature from a template](https://support.microsoft.com/en-us/office/create-an-email-signature-31fb24f9-e698-4789-b92a-f0e777f774ca).
- Microsoft has a specific Outlook 2010 guide that confirms personalized signatures can include text, images, logos, and handwritten-signature images: [Basic tasks in Outlook 2010](https://support.microsoft.com/en-us/office/basic-tasks-in-outlook-2010-9988b344-a7bf-4904-906f-414a3af7a727).
- Microsoft Q&A guidance identifies the classic desktop signature directory as `%UserProfile%\\AppData\\Roaming\\Microsoft\\Signatures`: [Missing signatures after W 365 and outlook updates](https://learn.microsoft.com/en-us/answers/questions/4553377/missing-signatures-after-w-365-and-outlook-updates).
- Microsoft distinguishes the New Outlook / web workflow, where signatures are managed from Settings and are not necessarily shared with the desktop Outlook signature store: [Create a signature and automatic reply](https://support.microsoft.com/en-us/outlook/officeweb/create-a-signature-and-automatic-reply).

## Compatibility boundary

| Client | Integration path | Scope |
|---|---|---|
| Classic Outlook 2010 | Local HTML/TXT/RTF signature package + user registry defaults | First implementation |
| Classic Outlook 2013 | Same local package strategy | First implementation |
| Classic Outlook 2016/2019/2021/Microsoft 365 | Same local package strategy, with version-aware registry handling | First implementation |
| New Outlook for Windows | Settings/cloud signature workflow | Separate follow-up |
| Outlook on the web | Settings > Mail > Compose and reply | Separate follow-up |

## Safety requirements

1. Never write to the machine-wide registry; use `HKCU` only.
2. Back up any existing signature files with the same name before replacing them.
3. Do not require administrator rights for the per-user signature folder.
4. Show the user that the installer targets Classic Outlook only.
