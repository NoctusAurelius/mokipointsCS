# How to Clear/Reset the Mokipoints Database

## Method 1: Delete Database Files (Complete Reset) ⭐ RECOMMENDED

This method completely removes all data and recreates the database from scratch.

### Steps:
1. **Stop the application** (stop debugging/close IIS Express)
2. **Close Visual Studio** (to release database file locks)
3. **Navigate to**: `mokipointsCS\App_Data\`
4. **Delete these files**:
   - `Mokipoints.mdf`
   - `Mokipoints_log.ldf`
5. **Restart the application**
6. **Database will be automatically recreated** with empty tables

### Using PowerShell (when app is stopped):
```powershell
# Navigate to project directory
cd "C:\Users\simon\source\repos\mokipointsCS\mokipointsCS\App_Data"

# Delete database files
Remove-Item "Mokipoints.mdf" -ErrorAction SilentlyContinue
Remove-Item "Mokipoints_log.ldf" -ErrorAction SilentlyContinue

# Verify deletion
Write-Host "Database files deleted. Restart application to recreate."
```

---

## Method 2: Delete All Data (Keep Database Structure)

This method keeps the database structure but removes all data.

### Using SQL Command:
1. **Stop the application**
2. **Open Command Prompt or PowerShell**
3. **Run these commands**:

```sql
sqlcmd -S "(LocalDB)\MSSQLLocalDB" -E -Q "USE Mokipoints; DELETE FROM TaskReviews; DELETE FROM TaskAssignments; DELETE FROM TaskObjectives; DELETE FROM Tasks; DELETE FROM FamilyMembers; DELETE FROM Families; DELETE FROM PointTransactions; DELETE FROM ChoreAssignments; DELETE FROM Chores; DELETE FROM OTPCodes; DELETE FROM Users;"
```

Or delete tables one by one (respecting foreign keys):
```sql
sqlcmd -S "(LocalDB)\MSSQLLocalDB" -E -Q "USE Mokipoints; DELETE FROM TaskReviews;"
sqlcmd -S "(LocalDB)\MSSQLLocalDB" -E -Q "USE Mokipoints; DELETE FROM TaskAssignments;"
sqlcmd -S "(LocalDB)\MSSQLLocalDB" -E -Q "USE Mokipoints; DELETE FROM TaskObjectives;"
sqlcmd -S "(LocalDB)\MSSQLLocalDB" -E -Q "USE Mokipoints; DELETE FROM Tasks;"
sqlcmd -S "(LocalDB)\MSSQLLocalDB" -E -Q "USE Mokipoints; DELETE FROM FamilyMembers;"
sqlcmd -S "(LocalDB)\MSSQLLocalDB" -E -Q "USE Mokipoints; DELETE FROM Families;"
sqlcmd -S "(LocalDB)\MSSQLLocalDB" -E -Q "USE Mokipoints; DELETE FROM PointTransactions;"
sqlcmd -S "(LocalDB)\MSSQLLocalDB" -E -Q "USE Mokipoints; DELETE FROM ChoreAssignments;"
sqlcmd -S "(LocalDB)\MSSQLLocalDB" -E -Q "USE Mokipoints; DELETE FROM Chores;"
sqlcmd -S "(LocalDB)\MSSQLLocalDB" -E -Q "USE Mokipoints; DELETE FROM OTPCodes;"
sqlcmd -S "(LocalDB)\MSSQLLocalDB" -E -Q "USE Mokipoints; DELETE FROM Users;"
```

---

## Method 3: Using Visual Studio Server Explorer

1. **Stop the application**
2. **Open Visual Studio**
3. **View → Server Explorer** (Ctrl+Alt+S)
4. **Expand Data Connections → Your connection**
5. **Right-click on tables → Delete** (one by one, respecting foreign key order)
6. **Or**: Right-click database → Delete (then restart app to recreate)

---

## Method 4: Detach and Delete Database

```sql
-- Detach database
sqlcmd -S "(LocalDB)\MSSQLLocalDB" -E -Q "EXEC sp_detach_db @dbname='Mokipoints'"

-- Then delete the .mdf and .ldf files manually
-- Restart application to recreate
```

---

## ⚠️ Important Notes

- **Always stop the application first** - Database files are locked while the app is running
- **Backup first** - Copy `.mdf` and `_log.ldf` files if you want to restore later
- **Method 1 is safest** - Complete reset ensures no orphaned data or constraint issues
- **After clearing** - Restart application and database will be recreated automatically

---

## Quick Reset Script (PowerShell)

Save this as `ResetDatabase.ps1` in your project root:

```powershell
# Stop IIS Express if running
Get-Process | Where-Object {$_.ProcessName -like "*iisexpress*"} | Stop-Process -Force

# Wait a moment for file locks to release
Start-Sleep -Seconds 2

# Navigate to App_Data
$appDataPath = "mokipointsCS\App_Data"
if (Test-Path $appDataPath) {
    # Delete database files
    Remove-Item "$appDataPath\Mokipoints.mdf" -ErrorAction SilentlyContinue
    Remove-Item "$appDataPath\Mokipoints_log.ldf" -ErrorAction SilentlyContinue
    Write-Host "Database files deleted successfully!" -ForegroundColor Green
    Write-Host "Restart your application to recreate the database." -ForegroundColor Yellow
} else {
    Write-Host "App_Data folder not found!" -ForegroundColor Red
}
```

Run with: `.\ResetDatabase.ps1`

