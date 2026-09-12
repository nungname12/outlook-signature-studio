# Choose the Windows GUI shell

Status: resolved
Type: decision
Map: outlook-signature-gui-map.md

## Question

Which Windows desktop GUI technology should the first real application use so the Guided Wizard remains attractive, works reliably with Outlook 2010 COM/local files, and can be deployed to helpdesk users?

Recommended starting point: C# WPF targeting a Windows-compatible .NET Framework/runtime. WPF gives the selected B layout room to grow while keeping direct access to Windows filesystem and registry APIs.

## Resolution

User selected the recommended C# WPF direction. The first implementation will keep the Guided Wizard interaction from the HTML MVP and use native Windows filesystem and registry access for Outlook integration.
