﻿(function () {
    'use strict';

    var app = angular.module('app');

    app.translationsCultureName = 'da';

    app.translations = {
        GENERAL: {
            COMING_SOON: 'Kommer snart',
            BUTTONS: {
                DONE: 'Udfør',
                CANCEL: 'Annuller',
                YES: 'Ja',
                NO: 'Nej',
            },
            LAYOUT: {
                TIME_REGISTRATION: 'Tidsregistrering',
                REPORTING: 'Rapportering',
                INVOICING: 'Fakturering',
                MY: 'Mig',
                LOG_OFF: 'Log af',
            },
            DATE_SELECT: {
                BACK: 'Tilbage',
                FORWARD: 'Fremad',
                SELECT: 'Vælg',
                TODAY: 'I dag',
                CHANGE_SCALE: 'Skift skala',
            },
            LOGO: {
                YOUR_LOGO: 'Dit logo',
                NOT_SUPPORTED: 'Logo ændringer supporteres ikke i din browser.',
                SIZE_ERROR: 'Billedet er for stort. Den maksimale størrelse er 500KB.',
            },
            CREATE_DROPDOWN: {
                CREATE_GROUP: 'Opret kunde',
                CHANGE_GROUP: 'Ændre kunde',
                SELECT_GROUP: 'Vælg kunde:',
                CREATE_TASK: 'Opret opgave',
                CHANGE_TASK: 'Ændre opgave',
                SELECT_TASK: 'Vælg opgave:',
            },
        },
        ERROR: {
            REQUIRED: 'Feltet er krævet.',
            MIN_LENGTH: 'Feltet er for kort.',
            MAX_LENGTH: 'Feltet er for langt.',
            MIN: 'Tallet er for lille.',
            MAX: 'Tallet er for stort.',
            NUMBER: 'Et validt tal er krævet.',
            WHOLENUMBER: 'Et heltal er krævet.',
            DATE: 'En valid dato er krævet.',
            TIME: 'Valid i mellem 0:00 og 24:00 timer.',
            CURRENCY: 'Et validt beløb er krævet.',
            PERCENTAGE: 'En validt procent tal er krævet.',
            SERVER_ERROR: 'Ukendt serverfejl opstod.',
        },
        TIME: {
            GROUP: 'Kunde',
            GROUP_PLACEHOLDER: 'Tilføj kunde',
            TASK: 'Opgave',
            TASK_PLACEHOLDER: 'Tilføj opgave',
            TIME: 'Tid',
            WEEK_TOTAL: 'Uge',
            MONTH_TOTAL: 'Måned',
            REMOVE: 'Fjern',

            ADD_GROUP: 'Tilføj kunde',
            ADD_TASK: 'Tilføj opgave',

            TIME_CALCULATOR: 'Tidsberegner',
            TIME_FROM: 'Tid fra',
            TIME_TO: 'til',
            BRAKE: 'Pause',
            TIME_RESULT: 'Tids resultat',
            BRAKE_00MIN: '0 minutter',
            BRAKE_05MIN: '5 minutter',
            BRAKE_10MIN: '10 minutter',
            BRAKE_15MIN: '15 minutter',
            BRAKE_30MIN: '30 minutter',
            BRAKE_45MIN: '45 minutter',
            BRAKE_60MIN: '1 time',
        },
        REPORT: {
            SORTING: 'Sorting',
            OVERVIEW: 'Rapport oversigt',
            CREATE_REPORT: 'Opret rapport',
            PERSON_TIME: 'Tid pr. person',
            NO_TIME_REGISTERED: 'Ingen tid registreret i perioden',
            GROUP: 'Kunde',
            TASK: 'Opgave',
            TIME: 'Tid',
            SELECT_USER: 'Vælg person',
            SELECT_GROUP: 'Vælg gruppe',
            SELECT_TASK: 'Vælg opgave',
            REPORT: 'Raport',
            NUMBER: 'Nummer',
            STATE: 'Status',
            STATE_CREATED: 'Sender',
            STATE_RESENDING: 'Gensender',
            STATE_SEND: 'Sendt',
            STATE_RESEND: 'Gensendt',
            CREATE_TIME: 'Tidspunkt',
            SEND_TO: 'Sendt til',
            REPORT_TIME: 'Tid',
            DOWNLOAD: 'Hent',
            VIEW_DETAILS: 'Se detaljer',
            NO_REPORTS: 'Ingen rapporter i perioden',
            NEW_REPORT: 'Ny rapport',
            REPORT_EMAIL: 'Rapport e-mail',
            EMAIL_SUBJECT: 'E-mail emne',
            EMAIL_SUBJECT_DFAULTTEXT: 'Timerapport',
            EMAIL_BODY: 'E-mail tekst',
            EMAIL_BODY_DFAULTTEXT: 'Hej,\n\nTimerapporten er vedhæftet som PDF fil.\n\nVenlig hilsen\n>> Tilføj dit navn <<',
            ATTACHED_PDF_TIME_REPORT: 'Vedhæftet PDF timerapport',
            ATTACHED_TIME_REPORT: 'Vedhæftet timerapport',
            REPORT_TITLE: 'Rapport titel',
            REPORT_TITLE_DFAULTTEXT: 'Timerapport',
            REPORT_NUMBER_ADDED: 'Rapport nummer tilføjes',
            REPORT_TITLE_SUB_TEXT: 'Rapport titel undertekst',
            REPORT_TITLE_SUB_DFAULTTEXT: 'Timerapport fra <begin_time> til <end_time>',
            YOUR_ORGANISATION: 'Din organisation',
            YOUR_ORGANISATION_ADDRESS: 'Your organisations adresse',
            REPORT_TEXT: 'Ekstra rapport tekst',
            SEND_TO_EMAIL: 'Send til e-mail (adskild e-mail adresser med komma)',
            SEND_REPORT: 'Send rapport',
            RESEND_REPORT: 'Gensend rapport',
            DELETE_REPORT: 'Slet rapport',
            DELETE_REPORT_CONFIRM: 'Er du sikker på at du ønsker at slette rapporten?',
        },
        INVOICE: {
            SORTING: 'Sorting',
            OVERVIEW: 'Faktura oversigt',
            CREATE_INVOICE: 'Opret faktura',
            NO_TIME_REGISTERED: 'Ingen tid registreret i perioden',
            GROUP: 'Kunde',
            TASK: 'Opgave',
            TIME: 'Tid',
            HOURPRICE: 'Timepris',
            PRICE: 'Pris',
            PERSON: 'Person',
            SELECT_USER: 'Vælg person',
            SELECT_GROUP: 'Vælg gruppe',
            SELECT_TASK: 'Vælg opgave',
            INVOICE: 'Faktura',
            NUMBER: 'Nummer',
            STATE: 'Status',
            STATE_CREATED: 'Sender',
            STATE_RESENDING: 'Gensender',
            STATE_SEND: 'Sendt',
            STATE_RESEND: 'Gensendt',
            SEND_TO: 'Sendt til',
            CUSTOMER: 'Kunde',
            DOWNLOAD: 'Hent',
            VIEW_DETAILS: 'Se detaljer',
            NO_INVOICES: 'Ingen fakturaer i perioden',
            SEND_INVOICE: 'Send faktura',
            NEW_INVOICE: 'Ny faktura',
            INVOICE_EMAIL: 'Faktura e-mail',
            SEND_TO_EMAIL: 'Send til e-mail (adskild e-mail adresser med komma)',
            EMAIL_SUBJECT: 'E-mail emne',
            EMAIL_SUBJECT_DFAULTTEXT: 'Faktura',
            EMAIL_BODY: 'E-mail tekst',
            EMAIL_BODY_DFAULTTEXT: 'Hej,\n\nFakturaen er vedhæftet som PDF fil.\n\nVenlig hilsen\n>> Tilføj dit navn <<',
            ATTACHED_PDF_INVOICE: 'Vedhæftet PDF faktura',
            ATTACHED_INVOICE: 'Vedhæftet faktura',
            INVOICE_TITLE: 'Faktura titel',
            INVOICE_TITLE_DFAULTTEXT: 'Faktura',
            INVOICE_NUMBER_ADDED: 'Fakturanummer tilføjes',
            INVOICE_TITLE_SUB_TEXT: 'Faktura titel undertekst',
            INVOICE_TITLE_SUB_DFAULTTEXT: 'Faktura fra <begin_time> til <end_time>',
            YOUR_ORGANISATION: 'Din organisation',
            YOUR_ORGANISATION_ADDRESS: 'Din organisations adresse',
            TO_CUSTOMER: 'Til kunde',
            VAT_NUMBER: 'Momsnummer',
            PAYMENT_DETAILS: 'Betalingsoplysninger',
            PAYMENT_DETAILS_LONG: 'Betalingsoplysninger, f.eks. dine bank og konto',
            INVOICE_TEXT: 'Ekstra faktura tekst',
            INVOICE_NUMBER: 'Fakturanummer',
            INVOICE_DATE: 'Fakturadato',
            PAYMENT_TERMS: 'Betalingsbetingelser',
            PAYMENT_TERMS_LONG: 'Betalingsbetingelser, f.eks. betales senest 10 dage efter faktureringsdatoen',
            PAYMENT_TERMS_DFAULTTEXT: 'Betales senest 14 dage efter faktureringsdatoen.',
            REFERENCE: 'Reference',
            REFERENCE_LONG: 'Kundereference',
            SUBTOTAL: 'Subtotal',
            TOTAL: 'I alt (DKK)',
            VAT: 'Moms',
            TAX: 'Skat',
            RESEND_INVOICE: 'Gensend faktura',
            CREDIT_NOTE: 'Kreditnota',
            SEND_CREDIT_NOTE: 'Send kreditnota',
            CREDIT_NOTE_EMAIL: 'Kreditnota e-mail',
            CREDIT_NOTE_EMAIL_SUBJECT_DFAULTTEXT: 'Kreditnota',
            CREDIT_NOTE_EMAIL_BODY_DFAULTTEXT: 'Hej,\n\nKreditnotaen er vedhæftet som PDF fil.\n\nVenlig hilsen\n>> Tilføj dit navn <<',
            ATTACHED_PDF_CREDIT_NOTE: 'Vedhæftet PDF kreditnota for faktura',
            DELETE_INVOICE: 'Slet faktura',
            DELETE_INVOICE_CONFIRM: 'Er du sikker på at du ønsker at slette fakturaen?',
        },
        CONFIG: {
            MY: 'Mig',
            ORGANIZATION: 'Organisation',
            USERS: 'Brugere',
        },
        MY: {
            FULL_NAME: 'Fulde navn',
            EMAIL: 'E-mail',      
            CHANGE_PASSWORD: 'Ændre kodeord',
            NEW_PASSWORD: 'Nyt kodeord',
        },
        ORG: {
            NAME: 'Navn',
            ADDRESS: 'Adresse',
            FIRSTINVOICENUMBER: 'Første fakturanummer',
            CHANGE_COUNTRY_AND_LANGUAGE: 'Ændre land og sprog',
            SELECT_COUNTRY_AND_LANGUAGE: 'Vælg land og sprog',
            CULTURES: {
                AUTO: 'Automatisk',
                ENGLISH_EU: 'Engelsk EU',
                ENGLISH_UK: 'Engelsk UK',
                ENGLISH_USA: 'Engelsk USA',
                DANISH: 'Dansk',
            },
        },
        USER: {
            CREATE_USER: 'Opret bruger',
            NAME: 'Navn',
            FULL_NAME: 'Fulde navn',
            EMAIL: 'E-mail',
            SEND_EMAIL: 'En e-mail sendes til den nye brugeres e-mail adresse.',
        },
        SUPPORT: {
            SUPPORT: 'Support',
            TOP_TEXT: 'Kontakt vores support så høre du fra os hurtigst muligt.',
            MESSAGE: 'Besked',
            SEND_MESSAGE: 'Send besked',
            CONFIRMATION_TEXT: 'Vi har modtaget din besked, du høre fra os.'
        }
    };

})();