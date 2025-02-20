-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Feb 20, 2025 at 09:25 AM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `db_samara`
--

-- --------------------------------------------------------

--
-- Table structure for table `tbl_class`
--

CREATE TABLE `tbl_class` (
  `class_id` char(10) NOT NULL,
  `name` varchar(50) NOT NULL,
  `user_id` varchar(20) NOT NULL,
  `teacher_id` char(10) NOT NULL,
  `registered_date` varchar(12) NOT NULL,
  `fee` float NOT NULL,
  `margin` float NOT NULL,
  `time` varchar(25) NOT NULL,
  `day` varchar(12) NOT NULL,
  `last_modified_date` varchar(12) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Triggers `tbl_class`
--
DELIMITER $$
CREATE TRIGGER `trg_classes_before_insert` BEFORE INSERT ON `tbl_class` FOR EACH ROW BEGIN
    DECLARE max_id INT;
    DECLARE new_id CHAR(10);

    -- Extract the numeric part of the last student_id
    SELECT IFNULL(MAX(CAST(SUBSTRING(class_id, 4) AS UNSIGNED)), 0) INTO max_id
    FROM tbl_class;

    -- Increment the numeric part and generate the new class_id
    SET max_id = max_id + 1;
    SET new_id = CONCAT('CLS', LPAD(max_id, 6, '0'));

    -- Assign the new_id to the student_id
    SET NEW.class_id = new_id;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_enrollment`
--

CREATE TABLE `tbl_enrollment` (
  `student_id` char(10) NOT NULL,
  `class_id` char(10) NOT NULL,
  `enrolled_date` varchar(12) NOT NULL,
  `user_id` varchar(20) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_log`
--

CREATE TABLE `tbl_log` (
  `log_id` int(11) NOT NULL,
  `time` varchar(10) NOT NULL,
  `type` varchar(20) NOT NULL,
  `user_id` varchar(20) NOT NULL,
  `description` varchar(50) DEFAULT NULL,
  `date` varchar(12) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_student`
--

CREATE TABLE `tbl_student` (
  `student_id` char(10) NOT NULL,
  `name` varchar(50) NOT NULL,
  `mobile` varchar(10) DEFAULT NULL,
  `whatsapp` varchar(10) DEFAULT NULL,
  `registered_date` varchar(12) NOT NULL,
  `user_id` varchar(20) NOT NULL,
  `last_modified_date` varchar(12) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Triggers `tbl_student`
--
DELIMITER $$
CREATE TRIGGER `trg_students_before_insert` BEFORE INSERT ON `tbl_student` FOR EACH ROW BEGIN
    DECLARE max_id INT;
    DECLARE new_id CHAR(10);

    -- Extract the numeric part of the last student_id
    SELECT IFNULL(MAX(CAST(SUBSTRING(student_id, 4) AS UNSIGNED)), 0) INTO max_id
    FROM tbl_student;

    -- Increment the numeric part and generate the new student_id
    SET max_id = max_id + 1;
    SET new_id = CONCAT('STD', LPAD(max_id, 6, '0'));

    -- Assign the new_id to the student_id
    SET NEW.student_id = new_id;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_teacher`
--

CREATE TABLE `tbl_teacher` (
  `teacher_id` char(10) NOT NULL,
  `name` varchar(50) NOT NULL,
  `user_id` char(10) NOT NULL,
  `registered_date` varchar(12) NOT NULL,
  `mobile` varchar(10) NOT NULL,
  `whatsapp` varchar(10) NOT NULL,
  `last_modified_date` varchar(12) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Triggers `tbl_teacher`
--
DELIMITER $$
CREATE TRIGGER `trg_teacher_before_insert` BEFORE INSERT ON `tbl_teacher` FOR EACH ROW BEGIN
    DECLARE max_id INT;
    DECLARE new_id CHAR(10);

    -- Extract the numeric part of the last teacher_id
    SELECT IFNULL(MAX(CAST(SUBSTRING(teacher_id, 4) AS UNSIGNED)), 0) INTO max_id
    FROM tbl_teacher;

    -- Increment the numeric part and generate the new teacher_id
    SET max_id = max_id + 1;
    SET new_id = CONCAT('TCR', LPAD(max_id, 6, '0'));

    -- Assign the new_id to the student_id
    SET NEW.teacher_id = new_id;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_user`
--

CREATE TABLE `tbl_user` (
  `user_id` varchar(20) NOT NULL,
  `name` varchar(50) NOT NULL,
  `mobile` varchar(10) DEFAULT NULL,
  `inserted_by` varchar(20) NOT NULL,
  `registered_date` varchar(12) NOT NULL,
  `user_role` varchar(10) NOT NULL,
  `password` varchar(60) NOT NULL,
  `is_disable` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `tbl_user`
--

INSERT INTO `tbl_user` (`user_id`, `name`, `mobile`, `inserted_by`, `registered_date`, `user_role`, `password`, `is_disable`) VALUES
('admin', 'Sineth', '7777777777', 'admin', '2024-12-29', 'Admin', '$2a$11$JjBoy1XbWW8fN9rVCqumPuzT/bmcOx/N5KLL0TcXv.od1qO2wkgTa', 1);

--
-- Indexes for dumped tables
--

--
-- Indexes for table `tbl_class`
--
ALTER TABLE `tbl_class`
  ADD PRIMARY KEY (`class_id`),
  ADD KEY `user_id` (`user_id`),
  ADD KEY `teacher_id` (`teacher_id`);

--
-- Indexes for table `tbl_enrollment`
--
ALTER TABLE `tbl_enrollment`
  ADD PRIMARY KEY (`student_id`,`class_id`),
  ADD KEY `class_id` (`class_id`),
  ADD KEY `user_id` (`user_id`);

--
-- Indexes for table `tbl_log`
--
ALTER TABLE `tbl_log`
  ADD PRIMARY KEY (`log_id`),
  ADD KEY `user_id` (`user_id`);

--
-- Indexes for table `tbl_student`
--
ALTER TABLE `tbl_student`
  ADD PRIMARY KEY (`student_id`),
  ADD KEY `user_id` (`user_id`);

--
-- Indexes for table `tbl_teacher`
--
ALTER TABLE `tbl_teacher`
  ADD PRIMARY KEY (`teacher_id`),
  ADD KEY `user_id` (`user_id`);

--
-- Indexes for table `tbl_user`
--
ALTER TABLE `tbl_user`
  ADD PRIMARY KEY (`user_id`),
  ADD KEY `inserted_by` (`inserted_by`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `tbl_log`
--
ALTER TABLE `tbl_log`
  MODIFY `log_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `tbl_class`
--
ALTER TABLE `tbl_class`
  ADD CONSTRAINT `tbl_class_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `tbl_user` (`user_id`),
  ADD CONSTRAINT `tbl_class_ibfk_2` FOREIGN KEY (`teacher_id`) REFERENCES `tbl_teacher` (`teacher_id`);

--
-- Constraints for table `tbl_enrollment`
--
ALTER TABLE `tbl_enrollment`
  ADD CONSTRAINT `tbl_enrollment_ibfk_1` FOREIGN KEY (`class_id`) REFERENCES `tbl_class` (`class_id`),
  ADD CONSTRAINT `tbl_enrollment_ibfk_2` FOREIGN KEY (`student_id`) REFERENCES `tbl_student` (`student_id`),
  ADD CONSTRAINT `tbl_enrollment_ibfk_3` FOREIGN KEY (`user_id`) REFERENCES `tbl_user` (`user_id`);

--
-- Constraints for table `tbl_log`
--
ALTER TABLE `tbl_log`
  ADD CONSTRAINT `tbl_log_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `tbl_user` (`user_id`);

--
-- Constraints for table `tbl_student`
--
ALTER TABLE `tbl_student`
  ADD CONSTRAINT `tbl_student_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `tbl_user` (`user_id`);

--
-- Constraints for table `tbl_teacher`
--
ALTER TABLE `tbl_teacher`
  ADD CONSTRAINT `tbl_teacher_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `tbl_user` (`user_id`);

--
-- Constraints for table `tbl_user`
--
ALTER TABLE `tbl_user`
  ADD CONSTRAINT `tbl_user_ibfk_1` FOREIGN KEY (`inserted_by`) REFERENCES `tbl_user` (`user_id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
