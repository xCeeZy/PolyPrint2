-- Создание тестовых данных для БД PolyPrint

-- Добавление пользователей
INSERT INTO Users (Login, Password, Role) VALUES ('admin', '123', 'Администратор');
INSERT INTO Users (Login, Password, Role) VALUES ('manager', '123', 'Менеджер');
INSERT INTO Users (Login, Password, Role) VALUES ('master', '123', 'Мастер');

-- Добавление клиентов
INSERT INTO Clients (Organization_Name, Contact_Name, Phone, Email) VALUES
('ООО "Техпринт"', 'Иванов Иван Иванович', '+7 (495) 123-45-67', 'info@techprint.ru'),
('ИП Петров', 'Петров Петр Петрович', '+7 (495) 234-56-78', 'petrov@mail.ru'),
('ООО "Полиграфия+"', 'Сидоров Сидор Сидорович', '+7 (495) 345-67-89', 'poly@poly.ru');

-- Добавление оборудования
INSERT INTO Equipment (Name, Model, Serial_Number, ID_Client, Condition) VALUES
('Принтер HP LaserJet', 'Pro M404dn', 'SN123456', 1, 'Исправно'),
('МФУ Canon', 'imageRUNNER 2425', 'CN789012', 1, 'Требует обслуживания'),
('Плоттер HP', 'DesignJet T650', 'HP345678', 2, 'Исправно');

-- Добавление заказов
INSERT INTO Orders (ID_Client, Order_Date, Status, Total) VALUES
(1, '2024-01-15', 'Выполнен', 50000),
(2, '2024-02-20', 'В работе', 35000),
(3, '2024-03-10', 'Новый', 120000);

-- Добавление сервисных заявок
INSERT INTO Service_Requests (ID_Client, ID_Equipment, Created_Date, Problem_Description, Status, ID_Master) VALUES
(1, 1, '2024-03-15', 'Замятие бумаги', 'Закрыта', 3),
(1, 2, '2024-03-20', 'Не печатает цветные документы', 'В работе', 3),
(2, 3, '2024-03-22', 'Требуется техническое обслуживание', 'Новая', NULL);
