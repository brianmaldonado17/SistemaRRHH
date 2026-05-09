CREATE DATABASE  IF NOT EXISTS `sirrhh` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `sirrhh`;
-- MySQL dump 10.13  Distrib 9.4.0, for Win64 (x86_64)
--
-- Host: localhost    Database: sirrhh
-- ------------------------------------------------------
-- Server version	9.4.0

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `ausencias`
--

DROP TABLE IF EXISTS `ausencias`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ausencias` (
  `id_ausencia` int NOT NULL AUTO_INCREMENT,
  `id_empleado` int DEFAULT NULL,
  `fecha_ausencia` date NOT NULL,
  `motivo` varchar(100) DEFAULT NULL,
  `descuenta_salario` tinyint(1) DEFAULT '1',
  `estado` varchar(20) NOT NULL DEFAULT 'Activo',
  PRIMARY KEY (`id_ausencia`),
  KEY `id_empleado` (`id_empleado`),
  CONSTRAINT `ausencias_ibfk_1` FOREIGN KEY (`id_empleado`) REFERENCES `empleados` (`id_empleado`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ausencias`
--

LOCK TABLES `ausencias` WRITE;
/*!40000 ALTER TABLE `ausencias` DISABLE KEYS */;
/*!40000 ALTER TABLE `ausencias` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `departamentos`
--

DROP TABLE IF EXISTS `departamentos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `departamentos` (
  `id_departamento` int NOT NULL AUTO_INCREMENT,
  `nombre_departamento` varchar(100) NOT NULL,
  `estado` varchar(20) DEFAULT 'Activo',
  PRIMARY KEY (`id_departamento`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `departamentos`
--

LOCK TABLES `departamentos` WRITE;
/*!40000 ALTER TABLE `departamentos` DISABLE KEYS */;
INSERT INTO `departamentos` VALUES (1,'Innovación y Desarrollo','Activo'),(2,'Ventas','Activo'),(3,'Recursos Humanos','Activo'),(4,'Compras','Activo'),(5,'Auditoría','Activo'),(6,'Publicidad','Activo'),(7,'Contabilidad','Activo');
/*!40000 ALTER TABLE `departamentos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `detalle_nominas`
--

DROP TABLE IF EXISTS `detalle_nominas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `detalle_nominas` (
  `id_detalle` int NOT NULL AUTO_INCREMENT,
  `id_nomina` int DEFAULT NULL,
  `id_empleado` int DEFAULT NULL,
  `dias_trabajados` int NOT NULL,
  `bonificaciones` decimal(10,2) DEFAULT '0.00',
  `descuentos_igss` decimal(10,2) DEFAULT '0.00',
  `otras_deducciones` decimal(10,2) DEFAULT '0.00',
  `total_liquido` decimal(10,2) NOT NULL,
  PRIMARY KEY (`id_detalle`),
  KEY `id_nomina` (`id_nomina`),
  KEY `detalle_nominas_ibfk_2` (`id_empleado`),
  CONSTRAINT `detalle_nominas_ibfk_1` FOREIGN KEY (`id_nomina`) REFERENCES `nominas` (`id_nomina`),
  CONSTRAINT `detalle_nominas_ibfk_2` FOREIGN KEY (`id_empleado`) REFERENCES `empleados` (`id_empleado`)
) ENGINE=InnoDB AUTO_INCREMENT=73 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `detalle_nominas`
--

LOCK TABLES `detalle_nominas` WRITE;
/*!40000 ALTER TABLE `detalle_nominas` DISABLE KEYS */;
INSERT INTO `detalle_nominas` VALUES (57,11,1,30,250.00,893.55,725.00,17131.45),(58,11,7,30,250.00,434.70,250.00,8565.30),(59,11,8,30,250.00,338.10,150.00,6761.90),(60,11,9,30,250.00,241.50,50.00,4958.50),(61,11,10,16,133.33,175.17,0.00,3584.83),(62,12,1,30,250.00,893.55,725.00,17131.45),(63,12,2,8,66.67,128.80,0.00,2604.54),(64,12,3,8,66.67,64.40,0.00,1335.60),(65,12,4,8,66.67,83.72,0.00,1716.28),(66,12,5,30,250.00,338.10,150.00,6761.90),(67,12,6,1,8.33,19.32,0.00,389.01),(68,12,7,30,250.00,434.70,250.00,8565.30),(69,12,8,30,250.00,338.10,150.00,6761.90),(70,12,9,30,250.00,241.50,50.00,4958.50),(71,12,10,30,250.00,328.44,140.00,6581.56),(72,12,11,24,200.00,251.16,60.00,5088.84);
/*!40000 ALTER TABLE `detalle_nominas` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `empleados`
--

DROP TABLE IF EXISTS `empleados`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `empleados` (
  `id_empleado` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(100) NOT NULL,
  `apellido` varchar(100) NOT NULL,
  `fecha_ingreso` date NOT NULL,
  `id_puesto` int DEFAULT NULL,
  `id_departamento` int DEFAULT NULL,
  `estado` varchar(20) DEFAULT 'Activo',
  `fecha_base_indemnizacion` date DEFAULT NULL,
  PRIMARY KEY (`id_empleado`),
  KEY `id_puesto` (`id_puesto`),
  KEY `id_departamento` (`id_departamento`),
  CONSTRAINT `empleados_ibfk_1` FOREIGN KEY (`id_puesto`) REFERENCES `puestos` (`id_puesto`),
  CONSTRAINT `empleados_ibfk_2` FOREIGN KEY (`id_departamento`) REFERENCES `departamentos` (`id_departamento`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `empleados`
--

LOCK TABLES `empleados` WRITE;
/*!40000 ALTER TABLE `empleados` DISABLE KEYS */;
INSERT INTO `empleados` VALUES (1,'Brian','Maldonado','2024-01-01',1,1,'Activo','2024-01-01'),(2,'Ivonne','García','2026-04-23',3,3,'Activo','2026-04-23'),(3,'Marco','Rodríguez','2026-04-23',4,5,'Activo','2026-04-23'),(4,'Kevin','Gil','2026-04-23',2,2,'Activo','2026-04-23'),(5,'Bryan','Torres','2026-04-01',5,4,'Activo','2026-04-01'),(6,'Carlos','Méndez','2026-04-24',11,7,'Activo','2026-04-24'),(7,'Laura','Gómez','2024-04-10',7,2,'Activo','2024-04-10'),(8,'Fernando','Rojas','2025-08-15',5,4,'Activo','2025-08-15'),(9,'Ana','López','2026-02-20',4,5,'Activo','2026-02-20'),(10,'Javier','Pérez','2026-03-16',6,6,'Activo','2026-03-16'),(11,'Lucía','Morales','2026-04-30',2,2,'Activo','2026-04-30');
/*!40000 ALTER TABLE `empleados` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `historial_bajas`
--

DROP TABLE IF EXISTS `historial_bajas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `historial_bajas` (
  `id_baja` int NOT NULL AUTO_INCREMENT,
  `id_empleado` int DEFAULT NULL,
  `fecha_baja` date NOT NULL,
  `motivo` varchar(50) NOT NULL,
  `observaciones` text,
  PRIMARY KEY (`id_baja`),
  KEY `historial_bajas_ibfk_1` (`id_empleado`),
  CONSTRAINT `historial_bajas_ibfk_1` FOREIGN KEY (`id_empleado`) REFERENCES `empleados` (`id_empleado`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `historial_bajas`
--

LOCK TABLES `historial_bajas` WRITE;
/*!40000 ALTER TABLE `historial_bajas` DISABLE KEYS */;
INSERT INTO `historial_bajas` VALUES (7,11,'2026-04-24','Renuncia Voluntaria','XDDD'),(8,6,'2026-04-24','Despido Justificado','fgasdfgsdfgdfsg');
/*!40000 ALTER TABLE `historial_bajas` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `historial_prestaciones`
--

DROP TABLE IF EXISTS `historial_prestaciones`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `historial_prestaciones` (
  `id_prestacion` int NOT NULL AUTO_INCREMENT,
  `id_empleado` int DEFAULT NULL,
  `tipo_prestacion` varchar(50) NOT NULL,
  `fecha_calculo` date NOT NULL,
  `monto_pagado` decimal(10,2) NOT NULL,
  `periodo_cubierto` varchar(100) DEFAULT NULL,
  `estado` varchar(20) DEFAULT 'Pagada',
  PRIMARY KEY (`id_prestacion`),
  KEY `historial_prestaciones_ibfk_1` (`id_empleado`),
  CONSTRAINT `historial_prestaciones_ibfk_1` FOREIGN KEY (`id_empleado`) REFERENCES `empleados` (`id_empleado`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `historial_prestaciones`
--

LOCK TABLES `historial_prestaciones` WRITE;
/*!40000 ALTER TABLE `historial_prestaciones` DISABLE KEYS */;
INSERT INTO `historial_prestaciones` VALUES (10,6,'Indemnizacion','2026-04-24',51287.67,'15/01/2022 al 24/04/2026','Pagada'),(11,2,'Bono 14','2026-04-24',27.40,'Julio 2025 - Junio 2026','Pagada'),(12,7,'Aguinaldo','2026-04-24',9000.00,'Diciembre 2025 - Noviembre 2026','Pagada');
/*!40000 ALTER TABLE `historial_prestaciones` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `indicadores_productividad`
--

DROP TABLE IF EXISTS `indicadores_productividad`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `indicadores_productividad` (
  `id_indicador` int NOT NULL AUTO_INCREMENT,
  `id_empleado` int DEFAULT NULL,
  `mes_anio` varchar(7) NOT NULL,
  `puntualidad` decimal(5,2) DEFAULT '0.00',
  `calidad_trabajo` decimal(5,2) DEFAULT '0.00',
  `puntuacion_desempeno` decimal(5,2) DEFAULT NULL,
  `metas_cumplidas` int DEFAULT NULL,
  PRIMARY KEY (`id_indicador`),
  KEY `indicadores_productividad_ibfk_1` (`id_empleado`),
  CONSTRAINT `indicadores_productividad_ibfk_1` FOREIGN KEY (`id_empleado`) REFERENCES `empleados` (`id_empleado`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `indicadores_productividad`
--

LOCK TABLES `indicadores_productividad` WRITE;
/*!40000 ALTER TABLE `indicadores_productividad` DISABLE KEYS */;
INSERT INTO `indicadores_productividad` VALUES (1,2,'2026-04',100.00,100.00,97.00,9),(2,4,'2026-04',100.00,100.00,100.00,10),(3,11,'2026-04',100.00,100.00,100.00,10);
/*!40000 ALTER TABLE `indicadores_productividad` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `nominas`
--

DROP TABLE IF EXISTS `nominas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `nominas` (
  `id_nomina` int NOT NULL AUTO_INCREMENT,
  `tipo_nomina` varchar(20) NOT NULL,
  `fecha_inicio` date NOT NULL,
  `fecha_fin` date NOT NULL,
  `total_pagado` decimal(12,2) DEFAULT '0.00',
  `estado` varchar(20) DEFAULT 'Generada',
  PRIMARY KEY (`id_nomina`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `nominas`
--

LOCK TABLES `nominas` WRITE;
/*!40000 ALTER TABLE `nominas` DISABLE KEYS */;
INSERT INTO `nominas` VALUES (11,'Mensual','2026-03-01','2026-03-31',41001.98,'Pagada'),(12,'Mensual','2026-04-01','2026-04-30',61894.88,'Pagada');
/*!40000 ALTER TABLE `nominas` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `puestos`
--

DROP TABLE IF EXISTS `puestos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `puestos` (
  `id_puesto` int NOT NULL AUTO_INCREMENT,
  `nombre_puesto` varchar(100) NOT NULL,
  `salario_base` decimal(10,2) NOT NULL,
  `estado` varchar(20) DEFAULT 'Activo',
  PRIMARY KEY (`id_puesto`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `puestos`
--

LOCK TABLES `puestos` WRITE;
/*!40000 ALTER TABLE `puestos` DISABLE KEYS */;
INSERT INTO `puestos` VALUES (1,'Programador Sr.',18500.00,'Activo'),(2,'Vendedor Jr.',6500.00,'Activo'),(3,'Analista RRHH',10000.00,'Activo'),(4,'Auditor Jr.',5000.00,'Activo'),(5,'Analista compras',7000.00,'Activo'),(6,'Publicista Jr.',6800.00,'Activo'),(7,'Vendedor Sr.',9000.00,'Activo'),(8,'Auditor Sr.',10000.00,'Activo'),(9,'Publicista Sr.',11000.00,'Activo'),(10,'Contador Jr.',4800.00,'Activo'),(11,'Contador Sr.',12000.00,'Activo');
/*!40000 ALTER TABLE `puestos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `roles`
--

DROP TABLE IF EXISTS `roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `roles` (
  `id_rol` int NOT NULL AUTO_INCREMENT,
  `nombre_rol` varchar(50) NOT NULL,
  `estado` varchar(20) DEFAULT 'Activo',
  PRIMARY KEY (`id_rol`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `roles`
--

LOCK TABLES `roles` WRITE;
/*!40000 ALTER TABLE `roles` DISABLE KEYS */;
INSERT INTO `roles` VALUES (1,'Administrador','Activo'),(2,'Colaborador','Activo'),(3,'RecursosHumanos','Activo');
/*!40000 ALTER TABLE `roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `usuarios`
--

DROP TABLE IF EXISTS `usuarios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usuarios` (
  `id_usuario` int NOT NULL AUTO_INCREMENT,
  `username` varchar(50) NOT NULL,
  `correo_electronico` varchar(150) NOT NULL,
  `password_hash` varchar(255) NOT NULL,
  `id_empleado` int DEFAULT NULL,
  `id_rol` int DEFAULT NULL,
  `estado` varchar(20) DEFAULT 'Activo',
  PRIMARY KEY (`id_usuario`),
  UNIQUE KEY `username` (`username`),
  KEY `id_rol` (`id_rol`),
  KEY `usuarios_ibfk_1` (`id_empleado`),
  CONSTRAINT `usuarios_ibfk_1` FOREIGN KEY (`id_empleado`) REFERENCES `empleados` (`id_empleado`),
  CONSTRAINT `usuarios_ibfk_2` FOREIGN KEY (`id_rol`) REFERENCES `roles` (`id_rol`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `usuarios`
--

LOCK TABLES `usuarios` WRITE;
/*!40000 ALTER TABLE `usuarios` DISABLE KEYS */;
INSERT INTO `usuarios` VALUES (1,'brian.maldonado','brian.maldonado17@gmail.com','$2a$12$DdS.0M0B/TS5TF/Xs8VVkuXPf2XN3UIbJZ4Zjnxygefp2SJEx5MwK',1,1,'Activo'),(2,'ivonne.garcia','ivonne_escoobar95@hotmail.es','$2a$11$sM/d5s8H3PZlZCdxpHPz.OkjCkXRbaBiCLRNkiiwYaWJdRtXftooS',2,3,'Activo'),(3,'carlos.mendez','carlos.mendez@gmail.com','$2a$11$y86dOX5myQ0Rk3jd24s/e.eTwzkiLM6uxjWGArTiOXgO23Y4urf5S',6,2,'Activo');
/*!40000 ALTER TABLE `usuarios` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping routines for database 'sirrhh'
--
/*!50003 DROP FUNCTION IF EXISTS `fn_CalcularPrestaciones` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`SIRRHH`@`localhost` FUNCTION `fn_CalcularPrestaciones`(p_id_empleado INT, p_tipo_prestacion VARCHAR(50)) RETURNS decimal(10,2)
    READS SQL DATA
BEGIN
    DECLARE v_salario DECIMAL(10,2);
    DECLARE v_fecha_ingreso DATE;
    DECLARE v_fecha_base_indem DATE; -- El nuevo reloj
    DECLARE v_estado VARCHAR(20);
    DECLARE v_fecha_baja DATE;
    DECLARE v_fecha_calculo DATE;
    
    DECLARE v_dias_totales INT;
    DECLARE v_faltas_totales INT DEFAULT 0;
    
    DECLARE v_dias_indemnizacion INT; -- Días específicos para indemnizar
    DECLARE v_faltas_indemnizacion INT DEFAULT 0;
    
    DECLARE v_dias_anuales INT;
    DECLARE v_faltas_anuales INT DEFAULT 0;
    
    DECLARE v_monto DECIMAL(10,2) DEFAULT 0.00;

    -- 1. Obtenemos el salario, fechas y estado (Incluyendo la nueva fecha base)
    SELECT p.salario_base, c.fecha_ingreso, c.estado, IFNULL(c.fecha_base_indemnizacion, c.fecha_ingreso) 
    INTO v_salario, v_fecha_ingreso, v_estado, v_fecha_base_indem
    FROM empleados c
    INNER JOIN puestos p ON c.id_puesto = p.id_puesto
    WHERE c.id_empleado = p_id_empleado;

    -- 2. Determinamos la fecha de corte
    IF v_estado = 'Inactivo' THEN
        SELECT fecha_baja INTO v_fecha_baja 
        FROM historial_bajas 
        WHERE id_empleado = p_id_empleado 
        ORDER BY fecha_baja DESC LIMIT 1;
        
        SET v_fecha_calculo = IFNULL(v_fecha_baja, CURDATE());
    ELSE
        SET v_fecha_calculo = CURDATE();
    END IF;

    -- ==============================================================
    -- 3A. DÍAS HISTÓRICOS GENERALES (Mantiene intactas las vacaciones)
    -- ==============================================================
    SET v_dias_totales = DATEDIFF(v_fecha_calculo, v_fecha_ingreso);

    SELECT COUNT(*) INTO v_faltas_totales 
    FROM ausencias 
    WHERE id_empleado = p_id_empleado AND descuenta_salario = TRUE
    AND estado = 'Activo' -- Validamos que la falta no esté anulada
    AND fecha_ausencia <= v_fecha_calculo;

    SET v_dias_totales = v_dias_totales - v_faltas_totales;

    -- ==============================================================
    -- 3B. DÍAS DE INDEMNIZACIÓN (Usa la nueva fecha base)
    -- ==============================================================
    SET v_dias_indemnizacion = DATEDIFF(v_fecha_calculo, v_fecha_base_indem);
    
    SELECT COUNT(*) INTO v_faltas_indemnizacion 
    FROM ausencias 
    WHERE id_empleado = p_id_empleado AND descuenta_salario = TRUE
    AND estado = 'Activo'
    AND fecha_ausencia BETWEEN v_fecha_base_indem AND v_fecha_calculo;
    
    SET v_dias_indemnizacion = v_dias_indemnizacion - v_faltas_indemnizacion;

    -- 4. Días del año actual (Para Bono 14 y Aguinaldo)
    IF (DATEDIFF(v_fecha_calculo, v_fecha_ingreso)) > 365 THEN
        SELECT COUNT(*) INTO v_faltas_anuales 
        FROM ausencias 
        WHERE id_empleado = p_id_empleado AND descuenta_salario = TRUE 
        AND estado = 'Activo'
        AND fecha_ausencia BETWEEN DATE_SUB(v_fecha_calculo, INTERVAL 1 YEAR) AND v_fecha_calculo;
        
        SET v_dias_anuales = 365 - v_faltas_anuales;
    ELSE
        SET v_dias_anuales = v_dias_totales;
    END IF;

    -- 5. Lógica de cálculo (Asignando el reloj correcto a cada uno)
    IF p_tipo_prestacion = 'Indemnizacion' THEN
        -- Indemnización usa su propio contador
        SET v_monto = (v_salario / 365) * v_dias_indemnizacion;
        
    ELSEIF p_tipo_prestacion IN ('Aguinaldo', 'Bono 14') THEN
        SET v_monto = (v_salario / 365) * v_dias_anuales;
        
    ELSEIF p_tipo_prestacion = 'Vacaciones' THEN
        -- Vacaciones sigue usando el contador histórico
        SET v_monto = ((v_salario / 30) * 15 / 365) * v_dias_totales;
    END IF;

    IF v_monto < 0 THEN SET v_monto = 0; END IF;

    RETURN v_monto;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_AltaEmpleado` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`SIRRHH`@`localhost` PROCEDURE `sp_AltaEmpleado`(
    IN p_nombre VARCHAR(100),
    IN p_apellido VARCHAR(100),
    IN p_fecha_ingreso DATE,
    IN p_id_puesto INT,
    IN p_id_departamento INT
)
BEGIN
    INSERT INTO empleados (nombre, apellido, fecha_ingreso, id_puesto, id_departamento, estado)
    VALUES (p_nombre, p_apellido, p_fecha_ingreso, p_id_puesto, p_id_departamento, 'Activo');
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_DarDeBajaEmpleado` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`SIRRHH`@`localhost` PROCEDURE `sp_DarDeBajaEmpleado`(
    IN p_id_empleado INT,
    IN p_motivo VARCHAR(50),
    IN p_observaciones TEXT
)
BEGIN
    -- 1. Cambiamos el estado del empleado a Inactivo en la tabla principal
    UPDATE empleados 
    SET estado = 'Inactivo' 
    WHERE id_empleado = p_id_empleado;

    -- 2. Registramos el movimiento en la bitácora de auditoría
    INSERT INTO historial_bajas (id_empleado, fecha_baja, motivo, observaciones)
    VALUES (p_id_empleado, CURDATE(), p_motivo, p_observaciones);

    -- 3. ¡REGLA DE SEGURIDAD NUEVA! 
    -- Buscamos si este empleado tenía un usuario de sistema y lo bloqueamos automáticamente
    UPDATE usuarios 
    SET estado = 'Inactivo' 
    WHERE id_empleado = p_id_empleado;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_GenerarNomina` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`SIRRHH`@`localhost` PROCEDURE `sp_GenerarNomina`(
    IN p_tipo_nomina VARCHAR(20),
    IN p_fecha_inicio DATE,
    IN p_fecha_fin DATE
)
BEGIN
    DECLARE v_id_nomina INT;
    DECLARE v_id_empleado INT;
    DECLARE v_salario_base DECIMAL(10,2);
    DECLARE v_fecha_ingreso DATE;
    DECLARE v_fecha_baja DATE;
    DECLARE v_inicio_real DATE;
    DECLARE v_fin_real DATE;
    DECLARE v_dias_a_pagar INT;
    DECLARE v_faltas INT;
    DECLARE v_igss DECIMAL(10,2);
    DECLARE v_isr DECIMAL(10,2);
    DECLARE v_bonificacion DECIMAL(10,2);
    DECLARE v_liquido DECIMAL(10,2);
    DECLARE v_total_nomina DECIMAL(12,2) DEFAULT 0.00;
    
    DECLARE done INT DEFAULT FALSE;
    DECLARE cur_empleados CURSOR FOR 
        SELECT id_empleado, p.salario_base, c.fecha_ingreso, 
               (SELECT fecha_baja FROM historial_bajas WHERE id_empleado = c.id_empleado ORDER BY fecha_baja DESC LIMIT 1) as fecha_baja
        FROM empleados c
        INNER JOIN puestos p ON c.id_puesto = p.id_puesto
        WHERE c.estado = 'Activo' 
           OR (c.estado = 'Inactivo' AND EXISTS (SELECT 1 FROM historial_bajas hb WHERE hb.id_empleado = c.id_empleado AND hb.fecha_baja BETWEEN p_fecha_inicio AND p_fecha_fin));
        
    DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = TRUE;

    INSERT INTO nominas (tipo_nomina, fecha_inicio, fecha_fin, total_pagado)
    VALUES (p_tipo_nomina, p_fecha_inicio, p_fecha_fin, 0);
    SET v_id_nomina = LAST_INSERT_ID();

    OPEN cur_empleados;
    read_loop: LOOP
        FETCH cur_empleados INTO v_id_empleado, v_salario_base, v_fecha_ingreso, v_fecha_baja;
        IF done THEN LEAVE read_loop; END IF;

        -- 1. LÓGICA DE CRUCE DE FECHAS REALES
        SET v_inicio_real = GREATEST(p_fecha_inicio, v_fecha_ingreso);
        SET v_fin_real = LEAST(p_fecha_fin, IFNULL(v_fecha_baja, p_fecha_fin));

        -- 2. DÍAS CALENDARIO BRUTOS
        SET v_dias_a_pagar = DATEDIFF(v_fin_real, v_inicio_real) + 1;
        
        IF v_dias_a_pagar < 0 THEN SET v_dias_a_pagar = 0; END IF;

        -- ==============================================================
        -- 3. AJUSTE DE MES COMERCIAL (LEY DE GUATEMALA - 30 DÍAS)
        -- ==============================================================
        IF v_dias_a_pagar > 0 AND v_inicio_real = p_fecha_inicio AND v_fin_real = p_fecha_fin THEN
            IF p_tipo_nomina = 'Mensual' THEN
                SET v_dias_a_pagar = 30;
            ELSEIF p_tipo_nomina = 'Quincenal' THEN
                SET v_dias_a_pagar = 15;
            END IF;
        END IF;

        -- 4. DESCUENTO DE AUSENCIAS (Validando que el registro esté Activo)
        SELECT COUNT(*) INTO v_faltas FROM ausencias 
        WHERE id_empleado = v_id_empleado 
        AND descuenta_salario = TRUE 
        AND estado = 'Activo' 
        AND fecha_ausencia BETWEEN v_inicio_real AND v_fin_real;
        
        SET v_dias_a_pagar = GREATEST(v_dias_a_pagar - v_faltas, 0);

        -- 5. CÁLCULO DE SALARIO Y BONIFICACIÓN PROPORCIONAL
        SET v_liquido = (v_salario_base / 30) * v_dias_a_pagar;
        SET v_bonificacion = (250.00 / 30) * v_dias_a_pagar;
        
        -- 6. DEDUCCIONES (IGSS 4.83% e ISR Proyectado)
        SET v_igss = v_liquido * 0.0483;
        SET v_isr = IF(v_liquido > 4000, (v_liquido - 4000) * 0.05, 0);
        
        SET v_liquido = (v_liquido + v_bonificacion) - v_igss - v_isr;

        -- 7. GUARDAR DETALLE SÓLO SI HAY ALGO QUE PAGAR
        IF v_dias_a_pagar > 0 THEN
            INSERT INTO detalle_nominas (id_nomina, id_empleado, dias_trabajados, bonificaciones, descuentos_igss, otras_deducciones, total_liquido)
            VALUES (v_id_nomina, v_id_empleado, v_dias_a_pagar, v_bonificacion, v_igss, v_isr, v_liquido);
            
            SET v_total_nomina = v_total_nomina + v_liquido;
        END IF;
    END LOOP;
    CLOSE cur_empleados;

    UPDATE nominas SET total_pagado = v_total_nomina WHERE id_nomina = v_id_nomina;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-05-09 15:01:30
