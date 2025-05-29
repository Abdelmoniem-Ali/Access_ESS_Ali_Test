export enum DaysOfWeek {
    None = 0,
    Sunday = 1,
    Monday = 2,
    Tuesday = 4,
    Wednesday = 8,
    Thursday = 16,
    Friday = 32,
    Saturday = 64,
    Weekdays = 62,
    Weekends = 65,
    All = 127,
}

export enum Frequency {
    Daily = 0,
    Weekly = 1,
    Monthly = 2,
    Yearly = 3,
}

export enum RecurrencePatternType {
    // Summary:
    //     The recurrence pattern is determined using a specific day of the month and/or
    //     month of the year.
    Explicit = 0,
    //
    // Summary:
    //     The recurrence pattern is calculated and does not occur on a specific day
    //     of the month and/or month of the year.
    Calculated = 1,
}
