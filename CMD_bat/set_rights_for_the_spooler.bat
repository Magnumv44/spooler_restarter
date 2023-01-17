@echo off
chcp 65001
@echo Текущие права службы
sc sdshow spooler
@echo Устанавливаем права на запуск службы
sc sdset spooler D:(A;;0x30;;;WD)(A;;CCLCSWLOCRRC;;;AU)(A;;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;BA)(A;;CCLCSWRPWPDTLOCRRC;;;SY)S:(AU;FA;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;WD)
@echo Проверяем еще раз, чтобы убедиться, что права изменились
sc sdshow spooler
pause