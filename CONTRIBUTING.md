# CONTRIBUTING

## Потік роботи
1. Створюємо issue → гілка `feature/...` або `fix/...`.
2. Коміти з префіксами: `feat:`, `fix:`, `docs:`, `test:`, `refactor:`.
3. Перед PR:
   - `dotnet format`
   - `dotnet build -c Release`
   - `dotnet test -c Release`

## Branch protection
- PR required + хоча б 1 review
- Після першого зеленого CI — додати required checks
