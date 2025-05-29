// eslint-disable-next-line prettier/prettier
/* eslint-disable prettier/prettier */

import moment from "moment";
import { BaseResponse } from "../Models/BaseResponse";
import { TaskInfo } from "../Models/TaskInfo";
import { SharedDiaryInfo } from "../Models/SharedDiaryInfo";
import { DiaryEventInfo } from "../Models/DiaryEventInfo";

export class MultiUserDiaryService {
    private _getToken: () => Promise<string>;
    private _apiUrl;
    private _softwareKey;

    constructor(getToken:() => Promise<string>, apiUrl: string, softwareKey: string){
        this._getToken = getToken;
        this._apiUrl = apiUrl;
        this._softwareKey = softwareKey;

        Date.prototype.toJSON = function () {
            return moment(this).format("YYYY-MM-DDTHH:mm:ss");
        };
    }

    private async getRequestInit<P>( method: string, payload?: P){
        const token = await this._getToken();
        const contentType: string = (method == "POST"?'application/json': '' );
        return { method: method,
        headers: {Authorization: `Bearer ${token}`,
                'Content-Type': contentType,
                'softwareKey': this._softwareKey},
        body: (payload?JSON.stringify(payload): null)}
    }

    public async getRegistrySettings(): Promise<BaseResponse<string[]>>{
        let responsePromise: Promise<BaseResponse<string[]>> | null = null;
        await fetch(this._apiUrl + "MultiUserDiary/GetRegistrySettings",await this.getRequestInit("GET")
            ).then(function(response){
                responsePromise = response.json();
            });
            if(responsePromise){
                return Promise.resolve(responsePromise);
            }
            else{
                return Promise.resolve({} as BaseResponse<string[]>);
            }
    }

    public async getTasks(): Promise<BaseResponse<TaskInfo[]>>{
        let responsePromise: Promise<BaseResponse<TaskInfo[]>> | null = null;
        await fetch(this._apiUrl + "MultiUserDiary/GetTasks",await this.getRequestInit("GET")
            ).then(function(response){
                responsePromise = response.json();
            });
            if(responsePromise){
                return Promise.resolve(responsePromise);
            }
            else{
                return Promise.resolve({} as BaseResponse<TaskInfo[]>);
            }
    }

    public async getOwners(): Promise<BaseResponse<SharedDiaryInfo[]>>{
        let responsePromise: Promise<BaseResponse<SharedDiaryInfo[]>> | null = null;
        await fetch(this._apiUrl + "MultiUserDiary/GetOwners",await this.getRequestInit("GET")
            ).then(function(response){
                responsePromise = response.json();
            });
            if(responsePromise){
                return Promise.resolve(responsePromise);
            }
            else{
                return Promise.resolve({} as BaseResponse<SharedDiaryInfo[]>);
            }
    }

    public async getDiaryEvents(startDate: Date, endDate: Date, owners : SharedDiaryInfo[]): Promise<BaseResponse<DiaryEventInfo[]>>{
        let responsePromise: Promise<BaseResponse<DiaryEventInfo[]>> | null = null;
        await fetch(this._apiUrl + "MultiUserDiary/GetDiaryEvents",await this.getRequestInit("POST",
            {StartDate: moment(startDate).toDate(), EndDate: moment(endDate).toDate(), Owners: owners}
            )).then(function(response){
                responsePromise = response.json();
            });
            if(responsePromise){
                return Promise.resolve(responsePromise);
            }
            else{
                return Promise.resolve({} as BaseResponse<DiaryEventInfo[]>);
            }
    }

    public async getTaskById(taskId: number): Promise<BaseResponse<TaskInfo>>{
        let responsePromise: Promise<BaseResponse<TaskInfo>> | null = null;
        await fetch(this._apiUrl + "MultiUserDiary/GetTaskInfo?pTaskId="+taskId,await this.getRequestInit("GET")
            ).then(function(response){
                responsePromise = response.json();
            });
            if(responsePromise){
                return Promise.resolve(responsePromise);
            }
            else{
                return Promise.resolve({} as BaseResponse<TaskInfo>);
            }
    }
};