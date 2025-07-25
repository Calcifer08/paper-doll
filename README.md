# Тестовое задание: Механика макияжа (Paper Doll)

Прототип мобильной игры, реализующий основную игровую механику нанесения макияжа. Проект был выполнен в качестве тестового задания для позиции Junior Unity Developer.

Основная задача заключалась в реализации интерактивного взаимодействия с UI-элементами (крем, тени, помада), управлении "рукой" персонажа и применении косметики на модель в соответствии с логикой, описанной в ТЗ.

---

### Демонстрация работы

[Демонстрация работы механики](https://disk.yandex.ru/i/K4WFd-GNHy2Y-Q)

---

### Основные возможности

- **Система вкладок:** Динамическая генерация предметов в инвентаре при переключении категорий (крем, тени и т.д.).
- **Механика "взятия" предмета:** Рука плавно перемещается к выбранному предмету, "берет" его и подносит к зоне взаимодействия.
- **Drag & Drop:** После взятия предмета игрок получает управление и может перетащить его на целевую зону.
- **Автоматические анимации:** При успешном применении косметики запускаются анимированные последовательности (например, "патрулирование" кисточкой по лицу).
- **Разные типы предметов:** Архитектура поддерживает как простые предметы (крем), так и составные (палетка теней и кисточка, где нужно сначала выбрать цвет).
- **Очистка макияжа:** Реализована логика для сброса макияжа при нажатии на спонж.

---

### Технические моменты реализации

Этот раздел описывает ключевые архитектурные и технические решения, принятые в ходе разработки.

#### 1. Архитектура на основе Абстракции и Делегирования

Для управления различными типами предметов была использована объектно-ориентированная архитектура, основанная на наследовании и делегировании ответственности.

- **Абстрактный класс `Item`:** Является базовым для всех интерактивных предметов. Он содержит общую логику (хранение исходной позиции, проверка пересечения с целью) и определяет абстрактные методы (`InteractWithHand`, `HandleDrop`, `ItemAction`), которые дочерние классы должны реализовать.
- **Конкретные реализации (`SimpleItem`, `CompositeItem`):** Уникальная логика для каждого типа предметов инкапсулирована в классах-наследниках.
  - `SimpleItem` реализует простую последовательность: "подлететь -> взять предмет -> поднести" и т.д.
  - `CompositeItem` реализует более сложную последовательность, включающую дополнительное движение для выбора цвета с палетки: "подлететь -> взять предмет -> подлететь к цвету -> окрасить предмет -> поднести" и т.д.
- **Разделение ответственности:** `HandController` выступает в роли "исполнителя", а `Item` — в роли "сценариста". `Item` диктует последовательность действий, вызывая публичные методы `HandController` (например, `MoveHand`), а `HandController` управляет физическим перемещением, состоянием (свободен/занят) и обработкой пользовательского ввода (`OnDrag`).

#### 2. Паттерн "Стратегия" для действий (`ScriptableObject`)

Чтобы избежать создания множества классов для предметов с одинаковой механикой, но разным визуальным эффектом (например, помада, тени, румяна — все они красят, а крем прячет прыщи), был применен паттерн "Стратегия".

- **Абстрактный `DraggableItemActionSO`:** Создан `ScriptableObject`, который определяет логику действия.
- **Конкретные "Стратегии" (`HideAcneActionSO`, `PaintActionSO`):** Каждое уникальное действие (спрятать прыщи, покрасить) вынесено в отдельный `ScriptableObject`.
- **Гибкость:** Класс `Item` имеет поле для `DraggableItemActionSO`. Это позволяет в инспекторе Unity назначать разные "действия" предметам, не меняя их основной код. Например, один и тот же `SimpleItem` может как прятать прыщи, так и красить губы, в зависимости от того, какой `Action` ему назначен.

#### 3. Управление через Корутины (Coroutines)

Все плавные перемещения и сложные последовательные/параллельные действия реализованы с помощью корутин.

- **Универсальный "двигатель" `MoveHand`:** Создана одна универсальная корутина для плавного перемещения руки, которая используется во всех сценариях.
- **Построение сложных сценариев:** Последовательности действий строятся путем последовательного вызова корутин.
- **Параллельные анимации:** Для одновременного движения руки ("патрулирование") и выполнения действия (исчезновение прыщей) запускаются две корутины параллельно, с последующим ожиданием их завершения через `yield return new WaitForSeconds()`.
