-- Table: public.DOCUMENT_INFO

-- DROP TABLE public."DOCUMENT_INFO";

CREATE TABLE IF NOT EXISTS public."DOCUMENT_INFO"
(
    "DOCUMENT_ID" uuid NOT NULL,
    "DESCRIPTION_TXT" text COLLATE pg_catalog."default",
    "FILE_SIZE_NBR" integer,
    "LANGUAGE_TXT" text COLLATE pg_catalog."default",
    "FILE_NAME_NM" text COLLATE pg_catalog."default",
    "DOCUMENT_URL" text COLLATE pg_catalog."default",
    "CORRELATION_ID" uuid,
    "DOCUMENT_TYPES" jsonb,
    "SUBMISSION_METHOD_CD" text COLLATE pg_catalog."default",
    "FILE_TYPE_CD" text COLLATE pg_catalog."default",
    "DATE_CREATED_DTE" date,
    "USER_CREATED_BY_ID" text COLLATE pg_catalog."default",
    "DATE_LAST_UPDATED_DTE" date,
    "USER_LAST_UPDATED_BY_ID" text COLLATE pg_catalog."default",
    "IS_DELETED_IND" boolean,
    "DATE_DELETED_DTE" date,
    "DELETED_BY_ID" text COLLATE pg_catalog."default",
    CONSTRAINT "XPKDocument_Info" PRIMARY KEY ("DOCUMENT_ID")
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public."DOCUMENT_INFO"
    OWNER to vesselregistrydbadmin;