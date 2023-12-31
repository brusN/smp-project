# Неизменяемые структуры данных (Persistent Data Structures)

## Реализация

Задание выполнено на языке C# с дополнительными требованиями 1, 2

## Требования

Реализуйте библиотеку со следующими структурами данных в persistent-вариантах:
- Массив (константное время доступа, переменная длинна)
- Двусвязный список
- Ассоциативный массив (на основе Hash-таблицы, либо бинарного дерева)

Требуется собственная реализация перечисленных структур. Найти соответствующие алгоритмы также является частью задания. Язык реализации не фиксируется, но рекомендуется Java/C#/C++. В базовом варианте решения все структуры данных могут быть сделаны на основе fat-node. 

> Должен быть единый API для всех структур, желательно использовать естественный API для выбранной платформы

## Дополнительные требования

1. Обеспечить произвольную вложенность данных (по аналогии с динамическими языками), не отказываясь при этом полностью от типизации посредством generic/template.
2. Реализовать универсальный undo-redo механизм для перечисленных структур с поддержкой каскадности (для вложенных структур)
3. Реализовать более эффективное по скорости доступа представление структур данных, чем fat-node
4. Расширить экономичное использование памяти на операцию преобразования одной структуры к другой (например, списка в массив)
5. Реализовать поддержку транзакционной памяти (STM)
