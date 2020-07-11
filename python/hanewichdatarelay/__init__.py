import uuid

class datapackage:
    stream_identifier: uuid.UUID
    stream_package_count: int
    package_number: int
    content: bytes

    def serialize(self):
        bs = []

        # Put in the UUID
        uuid_bytes = self.stream_identifier.bytes
        for b in uuid_bytes:
            bs.append(b)


        # put in stream package count
        spc_bytes = bytes(self.stream_package_count)
        for b in spc_bytes:
            bs.append(b)


        # put in package number
        pn_bytes = bytes(self.package_number)
        for b in pn_bytes:
            bs.append(b)

        # put in content
        for b in self.content:
            bs.append(b)

        return bs

    def deserialize(self, _bytes: bytes):

        #stream identifier
        si_bytes = []
        si_bytes.append(_bytes[0])
        si_bytes.append(_bytes[1])
        si_bytes.append(_bytes[2])
        si_bytes.append(_bytes[3])
        si_bytes.append(_bytes[4])
        si_bytes.append(_bytes[5])
        si_bytes.append(_bytes[6])
        si_bytes.append(_bytes[7])
        si_bytes.append(_bytes[8])
        si_bytes.append(_bytes[9])
        si_bytes.append(_bytes[10])
        si_bytes.append(_bytes[11])
        si_bytes.append(_bytes[12])
        si_bytes.append(_bytes[13])
        si_bytes.append(_bytes[14])
        si_bytes.append(_bytes[15])
        self.stream_identifier = uuid.UUID(si_bytes)


        #stream package count
        spc_bytes = []
        spc_bytes.append(_bytes[16])
        spc_bytes.append(_bytes[17])
        spc_bytes.append(_bytes[18])
        spc_bytes.append(_bytes[19])
        self.stream_package_count = int(spc_bytes)

        #package number
        pn_bytes = []
        pn_bytes.append(_bytes[20])
        pn_bytes.append(_bytes[21])
        pn_bytes.append(_bytes[22])
        pn_bytes.append(_bytes[23])
        self.package_number = int(pn_bytes)

        #content
        content_bytes = []
        t = 24
        while t < _bytes.count:
            content_bytes.append(_bytes[t])
            t = t + 1
        self.content = content_bytes



d = datapackage()
d.content = bytes("Hello world!", "utf-8")
d.package_number = 1
d.stream_package_count = 10
d.stream_identifier = uuid.uuid1()

bsss = d.serialize()

dp2 = datapackage()
dp2.deserialize(bsss)

print("Done")