import { commonPerFactory } from "src/app/_core/_helper/common-per-factory";

export const environment = {
  production: false,
  enableDebug: true,
  INK_SYSTEM_CODE: commonPerFactory.INK_SYSTEM_CODE,
  apiUrlEC: commonPerFactory.apiUrlEC,
  apiUrl: commonPerFactory.apiUrl,
  hub: commonPerFactory.hub,
};