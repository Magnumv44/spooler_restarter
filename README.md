![.NET 6.0](https://img.shields.io/badge/.NET-6.0-brightgreen)
# Spooler restarter
Альтернативна утиліта для перезапуску служби "<strong>Диспетчер друку</strong>", після закінчення своєї роботи робиться запис до системного журналу Windows, в розділ "<strong>Aplications</strong>".

## Структура проекту
1. [Spooler - основний застосунок для роботи зі службою "Диспетчер друку"](/Spooler/Spooler/Program.cs "Натисніть щоб переглянути")
2. [SetServicePermissions - застосунок перевірки та надання доступу для зміни статуса служби "Диспетчер друку"](/SetServicePermissions/SetServicePermissions/Program.cs "Натисніть щоб переглянути")
3. [CMD bat - набір команд для надання доступу для зміни статуса служби "Диспетчер друку"](/CMD_bat/set_rights_for_the_spooler.bat "Натисніть щоб переглянути")

## Спосіб використання
Перед початком використання <strong>Spooler</strong>, необхідно за допомогою [<strong>bat файлу</strong>](/CMD_bat/set_rights_for_the_spooler.bat "Натисніть щоб переглянути"), чи застосунка [<strong>SetServicePermissions</strong>](/SetServicePermissions/SetServicePermissions/Program.cs "Натисніть щоб переглянути"), надати користувачу доступ до служби "<strong>Диспетчер друку</strong>" для зміни її стану. Запуск <strong>bat файлу</strong>, чи застосунка <strong>SetServicePermissions</strong> необхідно здійснювати з <strong>правами адміністратора</strong>.<br />
<strong>Spooler</strong> запускається як звичайна програма. Під час виконання своєї роботи, не виводить жодних повідомлень чи запитів до користувача, що дає змогу встановити її запуск в налаштуваннях служби.<br />
Для своєї роботи <strong>не потребує прав адмінастратора</strong> в системі, за умови, що попреденьо було надано доступ на зміну стану служби "<strong>Диспетчер друку</strong>".

## Надання доступу для зміни стану служби "Диспетчера друку" без прав адміністратора
Надання доступу можна зробити двома способами:<br />

1. За допомогою [bat](/CMD_bat/set_rights_for_the_spooler.bat "Натисніть щоб переглянути") файлу ([інструкція з описом](https://www.magnumblog.space/soft/restarting-spooler-service-without-administrator-rights "Натисніть щоб переглянути"))
2. За допомогою спеціального [застосунка](/SetServicePermissions/SetServicePermissions/Program.cs "Натисніть щоб переглянути") ([інструкція з описом](# "В процесі написання"))

## Компіляція
Процес компіляції не потребує жодних особливих умов.

## Додаткові ресурси
1. [Інконки - https://icons8.com](https://icons8.com "Натисніть щоб перейти")
