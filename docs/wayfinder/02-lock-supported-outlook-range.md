# Lock the supported Outlook range

Status: resolved
Type: decision
Map: outlook-signature-gui-map.md

## Question

Should the first `Set Signature` release target Outlook 2010 only, or auto-detect every Classic Outlook version from 2010 onward?

Recommended starting point: auto-detect Outlook 2010, 2013, and 2016/2019/2021/Microsoft 365 Classic, while explicitly showing that New Outlook is not handled by the local installer.

## Resolution

User selected the recommended Classic Outlook 2010+ auto-detection scope. New Outlook remains a separate cloud/settings integration.
