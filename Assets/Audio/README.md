# เสียงของเกม Bowling

วางไฟล์เสียงในโฟลเดอร์นี้แล้วลากใส่ช่องใน Inspector ได้เลย ระบบต่อสายรออยู่แล้วทุกช่อง
ช่องไหนยังว่างเกมก็เล่นได้ปกติ แค่ไม่มีเสียงช่องนั้น

## 1. เพลงพื้นหลัง

ไฟล์เพลงวางใน `Audio/Music/` แล้วเลือก GameObject ชื่อ **AudioManager** ในแต่ละซีน

- ซีน `Scene01` : ลาก `Music/game_bgm.mp3` ใส่ช่อง **Music Clip**

เพลงจะเล่นวนเองตอน Start ไม่ต้องเขียนโค้ดเพิ่ม

## 2. เสียงเอฟเฟกต์

ไฟล์เสียงวางใน `Audio/SFX/` แล้วลากใส่ช่องบน **AudioManager** ตัวเดียวกัน

| ช่องใน Inspector | ดังตอนไหน | ชื่อไฟล์ที่แนะนำ |
|------------------|-----------|------------------|
| `buttonClip` | ปุ่มกด | `SFX/ui_click.wav` |
| `rollClip` | ปล่อยลูกกลิ้ง | `SFX/roll.wav` |
| `pinClip` | ลูกชนพิน | `SFX/pin_hit.wav` |
| `strikeClip` | สไตรค์ | `SFX/strike.wav` |
| `gameOverClip` | จบ 10 เฟรม | `SFX/game_over.wav` |

## 3. ปรับเสียงในเกม

ปุ่ม **SETTINGS** หรือกด **Esc** เปิดหน้าตั้งค่า มีแถบเลื่อน Music กับ Sound FX
ค่าที่ตั้งถูกจำด้วย `PlayerPrefs` เปิดเกมใหม่ก็ยังอยู่ และใช้ร่วมกันทุกซีน

## 4. ตั้งค่าไฟล์เสียงใน Unity

เลือกไฟล์เสียงแล้วดู Inspector

- **เพลงยาว** : Load Type = `Streaming`, Compression = `Vorbis`
- **เสียงเอฟเฟกต์สั้น** : Load Type = `Decompress On Load`, Compression = `PCM` หรือ `ADPCM`

รองรับ `.wav` `.mp3` `.ogg` — เสียงเอฟเฟกต์แนะนำ `.wav` เพราะไม่มีดีเลย์ตอนเริ่มเล่น
