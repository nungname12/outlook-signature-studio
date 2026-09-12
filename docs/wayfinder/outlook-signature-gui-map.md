# Outlook Signature GUI Integration

Status: implementation-complete (native MVP build verified)

## Destination

โปรแกรม GUI บน Windows ที่ให้ผู้ใช้กรอกข้อมูล Signature แล้วกด `Set Signature` เพื่อสร้าง/ติดตั้ง Signature และตั้งค่าอัตโนมัติใน Classic Outlook 2010 ขึ้นไป โดยไม่ต้องแก้ไฟล์เอง

## Notes

- Planning map created with the Wayfinder approach.
- Existing HTML Local MVP and the chosen Guided Wizard (B) are the visual starting point.
- First release targets Classic Outlook for Windows; New Outlook and Outlook Web are separate work.
- Use the domain glossary in `CONTEXT.md` when discussing the model.
- User confirmed the recommended choices and explicitly deferred Logo/image support for the first release.
- Native C# WPF MVP is implemented under `src/SignatureStudio/` and built to `dist/SignatureStudio.exe`.
- The build uses the machine's .NET Framework 4.x compiler and WPF assemblies; the executable is per-user and does not require an installer or administrator rights.

## Decisions so far

- [Guided Wizard UI](../../signature-studio.html): the selected interaction is a three-step flow with live preview and export/install actions.
- [Classic Outlook compatibility research](../outlook-compatibility-research.md): the first integration path targets the local Signature store and version-aware user settings.
- [User-confirmed first-release scope](../../CONTEXT.md): C# WPF Portable GUI, Classic Outlook 2010+, Set Signature for New + Reply/Forward, no Logo in v1.

## Not yet specified

- End-to-end validation on a physical Outlook 2010 installation and final Windows/.NET deployment matrix.

## Out of scope

- Direct cloud synchronization for New Outlook or Outlook Web in the first release.
- Sending or modifying email messages.
- Organization-wide policy enforcement until deployment and branding requirements are decided.
- Logo/image support in the first release, deferred by user decision.

## Frontier

- [x] [Choose the Windows GUI shell](01-choose-windows-gui-shell.md) — C# WPF
- [x] [Lock the supported Outlook range](02-lock-supported-outlook-range.md) — Classic Outlook 2010+
- [x] [Define Set Signature behavior](03-define-set-signature-behavior.md) — New + Reply/Forward
- [x] [Choose deployment and asset strategy](04-choose-deployment-and-assets.md) — Portable per-user, no Logo in v1
