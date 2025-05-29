// eslint-disable-next-line prettier/prettier
/* eslint-disable prettier/prettier */

import { DiaryEventInfo } from "../Models/DiaryEventInfo";
import { DaysOfWeek, Frequency, RecurrencePatternType } from "../Models/Enums";
import { MessageInfo } from "../Models/MessageInfo";


export class UtilService {
    public static cloneObject<T>(sourceObject: T):T{
        return structuredClone<T>(sourceObject);
    }

    public static getErrorMessageInfo(errors: string): MessageInfo{
        return {header: 'Error',
                messageType: 'danger',
                primaryText: 'OK',
                message: errors,
                cancelText:''}
    }

    public static getSucessMessageInfo(message: string): MessageInfo{
        return {header: 'Confirmation',
                messageType: 'success',
                primaryText: 'OK',
                message: message,
                cancelText:''}
    }

    public static getStartRecord(pageIndex: number, pageSize: number): number{
        return (pageIndex - 1) * pageSize + 1;
    }

    public static getEndRecord(pageIndex: number, pageSize: number, totalRecords: number): number{
        return ((pageIndex - 1) * pageSize + pageSize) > totalRecords
            ? totalRecords
            : (pageIndex - 1) * pageSize + pageSize;
    }

    public static contrastColor(hexcolor: string): string {
        let color = hexcolor;
        color = color.substring(1); // remove #
        const r = parseInt(color.substr(0, 2), 16);
        const g = parseInt(color.substr(2, 2), 16);
        const b = parseInt(color.substr(4, 2), 16);
        const yiq = ((r * 299) + (g * 587) + (b * 114)) / 1000;

        return (yiq >= 128) ? 'black' : 'white';
    }

    
    public static convertFromInfragisticsToKendo(item: DiaryEventInfo) : string {

        let result: string[] = [];

        if (item.patternFrequency !== null) {

            result = [];
            switch (item.patternFrequency) {
                case Frequency.Daily:
                    if (item.patternDaysOfWeek === DaysOfWeek.Weekdays) {
                        result.push("FREQ=WEEKLY");
                    }
                    else {
                        result.push("FREQ=DAILY");
                    }
                    break;
                case Frequency.Weekly:
                    result.push("FREQ=WEEKLY");
                    break;
                case Frequency.Monthly:
                    result.push("FREQ=MONTHLY");
                    break;
                case Frequency.Yearly:
                    result.push("FREQ=YEARLY");
                    break;
                default:
                    break;
            }


            if (item.rangeLimit == 1) {
                result.push(`COUNT=${item.rangeMaxOccurrences}`);
            }
            else if (item.rangeLimit == 2) {
                const date = new Date(item.rangeEndDate);
                const pad = (number: number) => {
                    if (number < 10) {
                        return '0' + number;
                    }
                    return number;
                };
                result.push(`UNTIL=${date.getFullYear()}${("0" + (date.getMonth() + 1)).slice(-2)}${("0" + (date.getDate())).slice(-2)}T${pad(date.getUTCHours())}${pad(date.getUTCMinutes())}00Z`);//qqq date format
            }

            if (item.patternInterval > 1) {
                result.push(`INTERVAL=${item.patternInterval}`);
            }

            if (item.patternType == RecurrencePatternType.Calculated || item.patternFrequency == Frequency.Weekly || (item.patternFrequency == Frequency.Daily && item.patternDaysOfWeek == DaysOfWeek.Weekdays)) {
                const days: string[] = [];
                let dayPrefixForOccurrenceInMonth = "";
                if (item.patternOccurrenceOfDayInMonth > 0 && item.patternFrequency === Frequency.Monthly) {
                    dayPrefixForOccurrenceInMonth = (item.patternOccurrenceOfDayInMonth === 5 ? -1 : item.patternOccurrenceOfDayInMonth).toString();
                }
                else if (item.patternType == RecurrencePatternType.Calculated && item.patternDaysOfWeek === DaysOfWeek.All && item.patternFrequency === Frequency.Yearly)// e.g 1 September every year
                {
                    result.push(`BYSETPOS=${item.patternOccurrenceOfDayInMonth}`);
                    dayPrefixForOccurrenceInMonth = "";
                }
                else if (item.patternOccurrenceOfDayInMonth > 0 && item.patternFrequency === Frequency.Yearly) {
                    dayPrefixForOccurrenceInMonth = (item.patternOccurrenceOfDayInMonth === 5 ? -1 : item.patternOccurrenceOfDayInMonth).toString();
                }

                if (this.hasFlag(item.patternDaysOfWeek, DaysOfWeek.Monday)) {
                    days.push(`${dayPrefixForOccurrenceInMonth}MO`);
                }

                if (this.hasFlag(item.patternDaysOfWeek, DaysOfWeek.Tuesday)) {
                    days.push(`${dayPrefixForOccurrenceInMonth}TU`);
                }

                if (this.hasFlag(item.patternDaysOfWeek, DaysOfWeek.Wednesday)) {
                    days.push(`${dayPrefixForOccurrenceInMonth}WE`);
                }

                if (this.hasFlag(item.patternDaysOfWeek, DaysOfWeek.Thursday)) {
                    days.push(`${dayPrefixForOccurrenceInMonth}TH`);
                }

                if (this.hasFlag(item.patternDaysOfWeek, DaysOfWeek.Friday)) {
                    days.push(`${dayPrefixForOccurrenceInMonth}FR`);
                }

                if (this.hasFlag(item.patternDaysOfWeek, DaysOfWeek.Saturday)) {
                    days.push(`${dayPrefixForOccurrenceInMonth}SA`);
                }

                if (this.hasFlag(item.patternDaysOfWeek, DaysOfWeek.Sunday)) {
                    days.push(`${dayPrefixForOccurrenceInMonth}SU`);
                }

                if (days.length > 0) {
                    const dayString = days.join(",");
                    result.push(`BYDAY=${dayString}`);
                }
            }

            if (item.patternFrequency == Frequency.Yearly) {
                result.push(`BYMONTH=${item.patternMonthOfYear}`);
            }
            else if (item.patternFrequency == Frequency.Monthly && item.patternType == RecurrencePatternType.Explicit) {
                result.push(`BYMONTHDAY=${item.patternDayOfMonth}`);
            }
        }

        return result.length === 0 ? "" : "RRULE:" + result.join(";");
    }

    private static hasFlag(pFlags: DaysOfWeek, pFlag: DaysOfWeek) {
        return (pFlags & pFlag) !== 0;
    }
}


