INSERT INTO [dbo].[SystemConfiguration]([SystemConfigurationId],[ConfigKey],[ConfigValue],[DataType],[Description],[ReferenceType],[ReferenceCode],[IsActive],[CreatedBy],[CreationDate],[UpdatedBy],[UpdateDate])
	VALUES ('3c89763d-0926-4027-84e3-3e31a57ee6fb','EG002','{"scaleGroup": "INV-5", "table": "inv", "productField": "insumo", "conditionField": "cond_producto", "costField": "cost"}','JSON','Tabla para reporte de productos vencidos',NULL,NULL,1,'Admin','2026-02-10 11:03:34.7533333',NULL,NULL);

INSERT INTO [dbo].[SystemConfiguration] ([SystemConfigurationId],[ConfigKey],[ConfigValue],[DataType],[Description],[ReferenceType],[ReferenceCode],[IsActive],[CreatedBy],[CreationDate],[UpdatedBy],[UpdateDate])
	VALUES ('a14fdaa2-9a43-4051-959a-a63371094b20','EG001','96','INT','Valor maximo a aplicar un plan de accion','EnterpriseGrouping','EG001',1,'Admin','2026-01-20 14:58:09.3760000',NULL,NULL);
