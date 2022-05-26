# Notes - выпускная квалификационная работа "Разработка приложения умного персонального блокнота"


https://user-images.githubusercontent.com/106331061/170540629-1bffd169-6ee0-4c32-8956-c487b2dfd0d2.mp4


В результате анализа предметной области и анализа существующих приложений–аналогов был сформулирован список требований. Пользователь должен иметь возможность:
* добавлять новые заметки;
* удалить ненужные заметки;
* просматривать существующие заметки;
* редактировать существующие заметки;
* прикреплять к заметкам картинки/фотографии из памяти мобильного устройства;
* вставлять ссылки в заметки;
* создавать наброски и вставлять их в заметки;
* искать заметки по ключевым словам (символам) из названия и описания заметки;
* создавать категории для удобного разделения заметок;
* переключаться между светлой и тёмной темой приложения для удобства;
* поделиться заметкой;
* составлять списки дел;
* изменять шрифт выбранного текста;
* изменять размер выбранного текста;
* изменять цвет выбранного текста;
* создавать нумерованные и ненумерованные списки;
* подчёркивать выбранный текст;
* выделять текст жирным шрифтом;
* зачёркивать выбранный текст;
* делать выбранный текст курсивным;
* отступить абзац;
* отменить любые модификации в тексте;
* заблокировать приложение с помощью ПИН-кода.

## Системные требования
Для разработки в Xamarin, требуются следующие версии программного обеспечения и пакетов SDK. Проверьте версию операционной системы (и убедитесь, что вы не используете Express-выпуск Visual Studio, в противном случае рекомендуется выполнить обновление до выпуска Community). В установщике Visual Studio 2019 и Visual Studio 2017 есть параметр для автоматической установки Xamarin (рабочая нагрузка Разработка мобильных приложений на .NET).
<table aria-label="Требования к Windows" class="table table-sm">
<thead>
<tr>
<th></th>
<th scope="col">Рекомендуется</th>
<th scope="col">Примечания</th>
</tr>
</thead>
<tbody>
<tr>
<th scope="row">Операционная система</th>
<td>Windows 10</td>
<td>Минимальная версия операционной системы — Windows 7. Для включения поддержки универсальной платформы Windows в Xamarin.Forms требуется Windows 10.</td>
</tr>
<tr>
<th scope="row">Xamarin.Android</th>
<td>Android 6.0 / API уровня 23</td>
<td>При необходимости вы можете создавать приложения для старых версий Android, используя последнюю версию пакета SDK, или выполняя сборку со старыми версиями пакета SDK.</td>
</tr>
</tbody>
</table>
Для тестирования приложения Xamarin.Forms можно развертывать на соответствующих устройствах и платформах.
