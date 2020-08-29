import { NowRequest, NowResponse } from "@vercel/node";
import azure from "azure-storage";
import _ from "lodash";
import storage from "./storage";

export default function (req: NowRequest, res: NowResponse) {
  const query = new azure.TableQuery();

  storage.queryEntities("ratesrm", query, null, function (
    error,
    result,
    response
  ) {
    if (error) {
      console.error(error);
      return res.send("Unexpected error occurred");
    }

    const dtos = _(result.entries)
      .map(convertTableEntityToDto)
      .orderBy((x) => x["order"])
      .value();

    console.log(`Received ${dtos.length} values`);
    res.send({
      rates: dtos,
    });
  });
}

function convertTableEntityToDto(entity) {
  const dto = {};
  for (const key in entity) {
    const propName = _.camelCase(key);
    dto[propName] = entity[key]._;
  }
  return dto;
}
