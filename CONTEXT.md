# Signature Studio domain glossary

## Signature

ข้อความและข้อมูลติดต่อที่ผู้ส่งต้องการให้ปรากฏท้ายอีเมล เช่น ชื่อ แผนก บริษัท เบอร์โทรศัพท์ อีเมล และเว็บไซต์

## Template

รูปแบบ Signature ที่ตั้งชื่อไว้เพื่อเลือกใช้ซ้ำ เช่น IT Helpdesk, Sales Team หรือ Management

## Sender profile

ชุดข้อมูลของผู้ส่งหนึ่งคนหรือหนึ่งทีมที่ใช้เติมลงใน Template

## Default rule

กติกาที่ระบุว่า Template ใดจะถูกใช้โดยอัตโนมัติกับอีเมลใหม่ หรือกับ Reply/Forward

## Classic Outlook

Outlook Desktop แบบดั้งเดิมที่จัดการ Signature บนเครื่องของผู้ใช้ และเป็นขอบเขตหลักของการเชื่อมต่อรุ่นแรก

## New Outlook

Outlook รุ่นใหม่ที่จัดการ Signature ผ่าน Settings/Cloud และมีขอบเขตการทำงานแยกจาก Classic Outlook

## Product decisions

- รุ่นแรกเป็นโปรแกรม Windows GUI แบบ Portable ต่อผู้ใช้
- ใช้ Guided Wizard เป็นประสบการณ์หลัก
- รองรับ Classic Outlook ตั้งแต่ 2010 ขึ้นไป โดยตรวจจับเวอร์ชันอัตโนมัติ
- `Set Signature` สร้างไฟล์ Signature ให้พร้อมใช้งาน; Outlook 2010 จะไม่เขียน Default ลง Registry เพื่อรักษาปุ่ม Signature ส่วน Office รุ่นใหม่กว่าตั้งค่า Default ให้ได้
- รุ่นแรกยังไม่รวม Logo หรือไฟล์รูปภาพประกอบ Signature
## Repository maintenance

- ทุกครั้งที่มีการอัปเดตหรือ Push ขึ้น GitHub ต้องตรวจให้มี `README.md` ที่ root และปรับเนื้อหาให้ตรงกับความสามารถล่าสุดของโปรเจค
