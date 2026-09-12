# Outlook Signature Studio

Native Windows GUI สำหรับสร้างและจัดการ Email Signature ให้กับ Classic Outlook บน Windows

## ความสามารถ

- เปิดโปรแกรมมาพร้อม Starter Draft ให้ User แก้ไขข้อมูลของตัวเอง
- แสดง Live Preview ของ Signature
- สร้างไฟล์ Signature แบบ HTML, TXT และ RTF
- รูปแบบชื่อ ตำแหน่ง และบริษัทอยู่บรรทัดเดียวกัน
- ไม่ใช้ HTML Table จึงไม่เปิดแถบ Table Tools ใน Outlook
- สำรองไฟล์ Signature เดิมก่อนเขียนทับ
- ทำงานแบบ Per-user ไม่ต้องใช้ Administrator
- รองรับ Outlook 2010 ขึ้นไปในโหมด Classic

## วิธีใช้งาน

1. เปิด [SignatureStudio.exe](dist/SignatureStudio.exe)
2. แก้ไขข้อมูลใน Starter Draft ให้เป็นข้อมูลของ User
3. กด `SET SIGNATURE`
4. ปิดและเปิด Outlook ใหม่

สำหรับ Outlook 2010 โปรแกรมจะสร้างไฟล์ Signature ให้พร้อมใช้งาน แต่จะไม่เขียนค่า Default Signature ลง Registry เพราะอาจทำให้ปุ่ม `Signature` บน Ribbon หาย ผู้ใช้เลือก Default เองครั้งเดียวที่:

`File > Options > Mail > Signatures`

## Compatibility

- Classic Outlook 2010
- Classic Outlook 2013
- Classic Outlook 2016
- Classic Outlook 2019
- Classic Outlook 2021
- Microsoft 365 Classic Outlook

New Outlook และ Outlook Web ใช้ระบบ Signature คนละแบบและไม่อยู่ในขอบเขตของรุ่นนี้

## Build จาก Source

ใช้ Windows PowerShell ในโฟลเดอร์โปรเจค:

```powershell
.\build.ps1
```

ไฟล์ผลลัพธ์จะอยู่ที่ `dist\SignatureStudio.exe`

## โครงสร้างโปรเจค

- `src/SignatureStudio/` — Source Code ของ Native WPF GUI
- `dist/` — ไฟล์โปรแกรมที่ Build แล้ว
- `docs/` — เอกสารและผลการศึกษาความเข้ากันได้
- `signature-studio.html` — Local MVP รุ่นเว็บ
- `build.ps1` — สคริปต์ Build

Logo และ Image Signature ยังไม่รวมใน v1 ตามขอบเขตที่กำหนดไว้
