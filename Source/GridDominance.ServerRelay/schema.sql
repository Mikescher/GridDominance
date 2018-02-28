DROP DATABASE IF EXISTS grid_dominance_relay;
CREATE DATABASE IF NOT EXISTS grid_dominance_relay;

USE grid_dominance_relay;

DROP TABLE IF EXISTS relay_log;
CREATE TABLE IF NOT EXISTS relay_log
(
  logdate           char(12)     NOT NULL,
  target            varchar(128) NOT NULL,
  logcount          int(11)      NOT NULL,

  PRIMARY KEY (logdate,target)
);