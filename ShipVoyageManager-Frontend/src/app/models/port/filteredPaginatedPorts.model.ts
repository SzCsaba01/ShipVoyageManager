import { IPort } from "./port.model";

export interface IFilteredPaginatedPorts {
    ports: IPort[];
    totalCount: number;
}