# 🧠 BreakFree — Habit Tracking System  
**Course:** Програмна інженерія  
**Team:** 5 students (cross-platform setup — 2 Mac users)  
**Language:** C#, .NET 8, WPF / ADO.NET / EF Core  
**Database:** SQLite  
**IDE:** Visual Studio / VS Code  

---

## 💡 Опис проєкту  
**BreakFree** — це кросплатформенний застосунок-трекер, який допомагає користувачам позбавлятися шкідливих звичок і формувати корисні.  
Програма дозволяє:
- відстежувати дні без зривів, прогрес і серії (streaks);
- переглядати аналітику успіхів;
- зберігати мотиваційні цитати та SOS-допомогу;
- бачити фінансову економію;
- отримувати підтримку під час кризових моментів.

---

## 🧩 Архітектура проєкту  
Реалізовано **трирівневу архітектуру (3-tier architecture)**:


---

## 🧱 База даних  
- **Тип:** SQLite  
- **Провайдер:** Microsoft.Data.Sqlite  
- **Скрипт:** [`db/create_breakfree.sql`](db/create_breakfree.sql)  
- **Основні таблиці:**  
  - `Users` — користувачі  
  - `Habits` — звички  
  - `DailyStatuses` — записи за день  
  - `Achievements`, `Savings`, `Quotes`, `SOSActions`, `UserSOSLogs`  

📊 **ER-діаграма:** `docs/ERD_BreakFree.jpeg`  
📄 **UML-опис:** `docs/BreakFree_Requirements.docx`

---

## ⚙️ Як запустити проєкт

### 1️⃣ Ініціалізація бази та сидинг (ADO.NET + Bogus)
```bash
cd src/BreakFree.ConsoleSeed
dotnet restore
dotnet run
